package jpeg;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.Rectangle;
import java.awt.RenderingHints;
import java.awt.color.ColorSpace;
import java.awt.image.BufferedImage;
import java.awt.image.ColorConvertOp;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Properties;
import java.util.Random;
import java.util.StringTokenizer;

import javax.swing.JFrame;

import jpeg.imagefiller.ImageFillingStrategy;
import jpeg.imagefiller.ImageFillingStrategyFactory;

import sun.awt.image.IntegerInterleavedRaster;

import com.sun.image.codec.jpeg.ImageFormatException;
import com.sun.image.codec.jpeg.JPEGCodec;
import com.sun.image.codec.jpeg.JPEGImageDecoder;
import com.sun.image.codec.jpeg.JPEGImageEncoder;
/**
 * TODO cleanup console output
 * TODO put replaced files in seperate folder and delete in case of exception
 * TODO use fakeimagegenerator for ftypo-based random image
 * done #1 rescale function for images that do not match the required aspect ratio (portrait/landscape problem) cut out some area of pic with right asp ratio.
 * @author possum
 *
 */
public class JpegTools {
	
	private boolean shuffleEnabled;
	private boolean fillUpMissingTilesWithImages;
	private final Random random = new Random();
	private Color backgroundColor;
	private ColorRangeContainer tileColors;
	// TODO props mit klasen namen fuellen, alias fuer classnames, 
	//- alias lesen, classname ermitteln, mit factory instazieren.
	private ImageFillingStrategy imageFiller;
	private int imageType;
	private List<File> deleteList;
	
	
	public JpegTools(Properties props) {
		if(props.containsKey("shuffleEnabled")) {
			shuffleEnabled = Boolean.parseBoolean(props.getProperty("shuffleEnabled"));
		}
		if(props.containsKey("fillUpMissingTilesWithImages")) {
			fillUpMissingTilesWithImages = Boolean.parseBoolean(props.getProperty("fillUpMissingTilesWithImages"));
		}
		if(props.containsKey("backgroundColor")) {
			backgroundColor = Color.decode(props.getProperty("backgroundColor"));
		}
		if(props.containsKey("tilesColorRange")) {
			parseTileColors(props);
		}
		if(props.containsKey("targetImageType")) {
			setImageType(props.getProperty("targetImageType"));
		}
		try {
			imageFiller = ImageFillingStrategyFactory.getInstance(props);
		} catch (ClassNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InstantiationException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		if(props.containsKey("placementBlur")) {
			int placementBlurPixels = Integer.parseInt(props.getProperty("placementBlur"));
			imageFiller.setPlacementBlurPixels(placementBlurPixels);
		}
		deleteList = new ArrayList<File>();
	}
	
	/**
	 * 
	create color ranges from props given
	*/
	private void parseTileColors(Properties props) {
		if(props.getProperty("tilesColorRange") == null ||
				props.getProperty("tilesColorRange").equals("")) {
			return;
		}
		// i.e. looks like this: "#8A3324-#CC7722,#8A3324-#CC7722"
		StringTokenizer tok = new StringTokenizer(
				props.getProperty("tilesColorRange"),
				","
		);
		if(!tok.hasMoreTokens()) {
			return;
		}
		tileColors = new ColorRangeContainer(random);
		
		while(tok.hasMoreTokens()) {
			//TODO proper regexp check
			String nextToken = tok.nextToken();
			String [] actualPair = nextToken.split("-");
			try {
				Color lower = Color.decode(actualPair[0]);
				Color upper = Color.decode(actualPair[1]);
				tileColors.add(new ColorRange(random, lower, upper));
			}
			catch(RuntimeException ex) {
				System.out.println("parameter 'tilesColorRange' in property file is corrupted.");
				throw ex;
			}
		}
	}

	/**
	 * 
	 * @param directoryWithJpegs where the images are
	 * @param mosaicFileName target image file
	 * @param tileDimensions width and height of source images in pixels
	 * @param aspectRatio aspect ratio of target image
	 * @throws IOException
	 */
	public void createMosaicJpegFromDirectory(
			File directoryWithJpegs,
			File mosaicFileName,
			Dimension tileDimensions,
			Dimension aspectRatio)
	throws IOException {
		
		if(directoryWithJpegs == null) {
			throw new NullPointerException("directoryWithJpegs is null.");
		}
		if(!directoryWithJpegs.exists()) {
			throw new FileNotFoundException(directoryWithJpegs+" does not exist.");
		}
		if(!directoryWithJpegs.isDirectory()) {
			throw new IOException("Directory '"+directoryWithJpegs.getAbsolutePath()+"' is no directory");
		}
		try {
			// correct probable format diversions/aspect ratio diversions in src images.
			ArrayList<File> filesToAdd = prepareImages(directoryWithJpegs, tileDimensions, aspectRatio);
			
			imageType = (filesToAdd.isEmpty()) ? BufferedImage.TYPE_INT_ARGB : getImageType(filesToAdd.get(0));
			
			// determine number of tiles available
			int tilesAvailable = filesToAdd.size();
			
			// calculate the aspectratio so that width*height=numberOfTiles
			Dimension realAspectRatio = calcAspectRatio(aspectRatio, tilesAvailable);
			
			//prepare image
			BufferedImage mosaic = prepareTarget(tilesAvailable, tileDimensions, realAspectRatio, imageType);
			
			//shuffle and fillup List
			prepareImageList(filesToAdd, realAspectRatio.height*realAspectRatio.width, tileDimensions);
			
			// fill target image with tiles
			fillImage(mosaic, tileDimensions, realAspectRatio, filesToAdd);
			
			// encode and write targetfile
			System.out.println("encoding image: "+mosaicFileName.getAbsolutePath());
			writeImageFile(mosaicFileName, mosaic);
			System.out.println("done enc.");
		}
		catch(RuntimeException ex) {
			System.out.println("Operation failed, cause: "+ex.getMessage());
			throw ex;
		}
		finally {
			// delete temp files
			System.out.println("Deleting temp files...");
			deleteTempFiles();
			System.out.println("Done.");
		}
	}
	
	private void deleteTempFiles() {
		for(File f : deleteList) {
			System.out.print("deleting: '"+f.getAbsolutePath());
			System.out.println("' success: "+f.delete());
		}
	}

	private int getImageType(File file) throws ImageFormatException, IOException {
			InputStream in = new FileInputStream(file);
			JPEGImageDecoder decoder = JPEGCodec.createJPEGDecoder(in);
			BufferedImage img = decoder.decodeAsBufferedImage();
			return img.getType();
	}

	private File writeImageFile(File imageFileName, BufferedImage image) throws ImageFormatException, IOException {
		FileOutputStream fout = new FileOutputStream(imageFileName);
		JPEGImageEncoder enc = JPEGCodec.createJPEGEncoder(fout);
		enc.encode(image);
		fout.close();
		return imageFileName;
	}

	private void prepareImageList(ArrayList<File> filesToAdd, int tilesNeeded, Dimension tileDimensions) throws ImageFormatException, IOException {
		if(shuffleEnabled) {
			Collections.shuffle(filesToAdd);
		}
		for(int i=0; filesToAdd.size() < tilesNeeded; i++) {
			if(fillUpMissingTilesWithImages) {
				filesToAdd.add(filesToAdd.get(i));
			}
			else {
				filesToAdd.add(null);
			}
		}
		if(shuffleEnabled) {
			Collections.shuffle(filesToAdd);
		}
	}


	private void fillImage(BufferedImage mosaic,
			Dimension tileDimensions, Dimension realAspectRatio,
			ArrayList<File> filesToAdd) throws ImageFormatException, IOException {
		System.out.println("fillImage: start");
		
		int currentIndex = 0;
		
		for(int yPos=0; yPos<realAspectRatio.height; yPos++) {
			for(int xPos=0; xPos<realAspectRatio.width; xPos++) {
				System.out.print("x="+(xPos*tileDimensions.width)+"\ty="+(yPos*tileDimensions.height)+"\t");
				
				Rectangle bounds = imageFiller.computeBoundsForNextTile(mosaic, tileDimensions, xPos, yPos);

				BufferedImage currentSubImage = mosaic.getSubimage(
						bounds.x,
						bounds.y,
						bounds.width,
						bounds.height
				);
				BufferedImage nextTile = loadImage(filesToAdd.get(currentIndex++));
				if(nextTile == null) {
					Graphics2D g = (Graphics2D) currentSubImage.getGraphics();
					g.setColor(getNextColor());
					g.fillRect(0, 0, tileDimensions.width, tileDimensions.height);
				}
				else {
					if(!(nextTile.getData() instanceof IntegerInterleavedRaster) ) {
						nextTile = convertColors(currentSubImage, nextTile);
					}
					currentSubImage.setData(
						nextTile.getData()	
					);
				}
			}
		}
		System.out.println("fillImage: done");
	}

	private Color getNextColor() {
		if(tileColors != null) {
			return tileColors.next();
		}
		return backgroundColor;
	}

	private BufferedImage convertColors(BufferedImage destination,
			BufferedImage toConvert) {
		//System.out.println("converting Colors..");
		ColorSpace destinationColors = destination.getColorModel().getColorSpace();
		ColorSpace sourceColors = toConvert.getColorModel().getColorSpace();
		RenderingHints hints = new RenderingHints(null);
		hints.put(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_BICUBIC);
		ColorConvertOp convertOp = new ColorConvertOp(sourceColors, destinationColors, hints);
		BufferedImage conversionResult = convertOp.filter(toConvert, destination);
		//System.out.println("converting done!");
		return conversionResult;
	}

	private BufferedImage loadImage(File image) throws ImageFormatException, IOException {
		if(image == null) {
			System.out.println();
			return null;
		}
		System.out.println("load: "+image.getAbsolutePath());
		InputStream in = new FileInputStream(image);
		JPEGImageDecoder decoder = JPEGCodec.createJPEGDecoder(in);
		BufferedImage img = decoder.decodeAsBufferedImage();
		in.close();
		return img;
	}

	/**
	 * // calculate the aspectratio so that width*height=numberOfTiles
	 * @param aspectRatio
	 * @param tileDimensions
	 * @param tilesAvailable
	 * @return
	 */
	private Dimension calcAspectRatio(Dimension aspectRatio, int tilesAvailable) {
		int minTiles = aspectRatio.width*aspectRatio.height;
		int dx = aspectRatio.width;
		int dy = aspectRatio.height;
		while(minTiles < tilesAvailable) {
			aspectRatio.width += dx;
			aspectRatio.height += dy;
			minTiles = aspectRatio.width*aspectRatio.height;
		}
		//now we have more of equal to tilesAvailable tiles.
		return aspectRatio;
	}

	/**
	 * 
	 * @param tiles
	 * @param tileDimensions
	 * @param imageType 
	 * @param aspectRatio
	 * @return
	 */
	private BufferedImage prepareTarget(int tiles,
			Dimension tileDimensions, Dimension realAspectRatio, int imageType) {
		BufferedImage target = null;
		System.out.println("prepareTarget: no of tiles: "+tiles);
		System.out.println("prepareTarget: realAspectRatio: "+realAspectRatio.toString());
		System.out.println("prepareTarget: tileDimensions="+tileDimensions);
		
		int targetWidth = realAspectRatio.width*tileDimensions.width;
		int targetHeight = realAspectRatio.height*tileDimensions.height;;
		System.out.print("prepareTarget: targetWidth="+targetWidth);
		System.out.println(", ("+realAspectRatio.width+"*"+tileDimensions.width+")");
		System.out.print("prepareTarget: targetHeight="+targetHeight);
		System.out.println(", ("+realAspectRatio.height+"*"+tileDimensions.height+")");
		target = new BufferedImage(targetWidth, targetHeight, imageType);
		Graphics2D g = (Graphics2D) target.getGraphics();
		g.setColor(backgroundColor);
		g.fillRect(0, 0, target.getWidth(), target.getHeight());
		return target;
	}

	private ArrayList<File> prepareImages(File directoryWithJpegs,
			Dimension tileDimensions, Dimension aspectRatio) throws ImageFormatException, IOException {
		
		// check dimensions
		// rescale if necessary
		
		File [] files = directoryWithJpegs.listFiles();
		ArrayList<File> fileList = new ArrayList<File>();
		for(int i=0; i<files.length; i++) {
			if(files[i].getAbsolutePath().endsWith(".jpg") || 
					files[i].getAbsolutePath().endsWith(".JPG")) {
				
				Image image = loadImage(files[i]);
				// rescale if necessary
				if( ((BufferedImage)image).getHeight() != tileDimensions.height || 
						((BufferedImage)image).getWidth() != tileDimensions.width) {
					System.out.println("rescale "+files[i].getAbsolutePath());
					// cut it right
					image = cutOut(image, tileDimensions);
					// rescale
					image = rescale(image, tileDimensions);
					File rescaledImageFile = new File(files[i].getAbsolutePath()+".rescaled.jpg");
					writeImageFile(rescaledImageFile, (BufferedImage)image);
					fileList.add(rescaledImageFile);
					deleteList.add(rescaledImageFile);
					System.out.println("rescale done.");
				}
				else {
					fileList.add(files[i]);
				}
			}
			
		}
		return fileList;
		
	}

	private Image rescale(Image image, Dimension tileDimensions) {
		if(tileDimensions == null) {
			throw new RuntimeException("tileDimensions is null");
		}
		else if(image == null) {
			throw new RuntimeException("image is null");
		}
		if(tileDimensions instanceof MosaickrDimensions) {
			// later features
			throw new RuntimeException("not suppoted yet.");
		}
		else {
			BufferedImage img = (BufferedImage) image;
			int newHeight = tileDimensions.height;
			int newWidth = tileDimensions.width;
			BufferedImage newImage = new BufferedImage(newWidth, newHeight, img.getType());
			
			RenderingHints hints = new RenderingHints(null);
			hints.put(RenderingHints.KEY_INTERPOLATION, RenderingHints.VALUE_INTERPOLATION_BICUBIC);

			Graphics2D g = (Graphics2D) newImage.getGraphics();
			g.setRenderingHints(hints);
	        g.drawImage(img, 0, 0, newWidth, newHeight, null);
	        return newImage;
		}
	}

	private Image cutOut(Image image, Dimension tileDimensions) {
		//System.out.println("Cutting...");
		BufferedImage img = (BufferedImage)image;
		double imageAspectRatio = ((double)img.getWidth())/((double)img.getHeight());
		double dimensionsAspectRatio = ((double)tileDimensions.width)/((double)tileDimensions.height);
		if(imageAspectRatio != dimensionsAspectRatio) {
			// zu breit
			if(imageAspectRatio > dimensionsAspectRatio) {
//				h(t)neu=h(i)
//				w(t)neu=w(t)alt*(h(t)neu/h(t)alt)
//				x_cutout=(w(i)/2)-(w(t)neu/2)
				final double w_t_old = tileDimensions.width;
				final double h_t_old = tileDimensions.height;
				double h_t_new = img.getHeight();
				double w_t_new = ( w_t_old * (h_t_new/h_t_old) );
				double x_cutout = (((double)img.getWidth())/2.) - (w_t_new/2.);
				/*System.out.println("image too wide" +
						", w_t_old="+w_t_old+
						", h_t_old="+h_t_old+
						", h_t_new="+h_t_new+
						", w_t_new="+w_t_new+
						", x_cutout"+x_cutout);*/
				image = ((BufferedImage)image).getSubimage((int)x_cutout, 0, (int)w_t_new, (int)h_t_new);
			}
			//zu hoch
			else {
//				w(t)neu=w(i)
//				h(t)neu=h(t)alt*(w(t)neu/w(t)alt)
//				y_cutout=(h(i)/2)-(h(t)neu/2)
				final double w_t_old = tileDimensions.width;
				final double h_t_old = tileDimensions.height;
				double w_t_new = (double)img.getWidth();
				double h_t_new = (h_t_old * (w_t_new/w_t_old) );
				double y_cutout = (((double)img.getHeight())/2.) - (h_t_new/2.);
				/*System.out.println("image too high" +
						", w_t_old="+w_t_old+
						", h_t_old="+h_t_old+
						", h_t_new="+h_t_new+
						", w_t_new="+w_t_new+
						", y_cutout"+y_cutout);*/
				image = ((BufferedImage)image).getSubimage(0, (int)y_cutout, (int)w_t_new, (int)h_t_new);
			}
		}
		else {
			//System.out.println("image ok, nothing todo");
		}
		//System.out.println("done Cutting...");
		return image;
	}

	private void setImageType(String imageTypeString) {
		if(imageTypeString.equals("RGB")) {
			imageType = BufferedImage.TYPE_INT_ARGB;
		}
		else if(imageTypeString.equals("BW")) {
			imageType = BufferedImage.TYPE_BYTE_GRAY;
		}
		else {
			imageType = BufferedImage.TYPE_INT_ARGB;
		}
	}
	
	public void createMosaicJpegFromDirectory(File directoryWithJpegs, File mosaicFileName) {
		this.createMosaicJpegFromDirectory(directoryWithJpegs, mosaicFileName);
	}
	
	public static void main(String [] args) {
		try {
			JpegTools jpegTools=new JpegTools(new Properties());
			
			Color c=new Color(jpegTools.random.nextInt(256), 
					jpegTools.random.nextInt(256), 
					jpegTools.random.nextInt(256));
			System.out.println("c="+c);
			BufferedImage image = new BufferedImage(320, 320, BufferedImage.TYPE_INT_ARGB);
			image.getGraphics().setColor(c);
			image.getGraphics().fillRect(0, 0, 320, 320);
			int len = image.getHeight()*image.getWidth();
			int [] rgb = new int[len];
			for(int i=0; i<len; i++) {
				rgb[i] = c.getRGB();
			}
			image.setRGB(0, 0, image.getWidth(), image.getHeight(), rgb, 0, 0);
			
			JFrame f = new JFrame();
			f.setSize(444, 444);
			JPanell p = new JPanell(image);
			p.setSize(444, 444);
			//p.prepareImage(image, p);
			
			f.add(p);
			f.setVisible(true);
			p.repaint();
			f.repaint();
			
		} catch (ImageFormatException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	public static void testJpegTools() throws ImageFormatException, IOException {
		InputStream in1 = new FileInputStream(new File("1.jpg"));
		InputStream in2 = new FileInputStream(new File("2.jpg"));
		JPEGImageDecoder decoder = JPEGCodec.createJPEGDecoder(in1);
		
		BufferedImage img1 = decoder.decodeAsBufferedImage();
		
		decoder = JPEGCodec.createJPEGDecoder(in2);
		BufferedImage img2 = decoder.decodeAsBufferedImage();
		
		BufferedImage imgDest = new BufferedImage(
				img1.getWidth(), img1.getHeight()*2, img1.getType()
			);
		imgDest.getSubimage(0, 0, img1.getWidth(), img1.getHeight()).setData(img1.getData());
		imgDest.getSubimage(0, img1.getHeight(), img2.getWidth(), img2.getHeight()).setData(img2.getData());
		
		
		//JPEGEncodeParam encodeParam = JPEGCodec.getDefaultJPEGEncodeParam(decoder.getJPEGDecodeParam());
		//encodeParam.setQuality(0.8f, false);
		//encodeParam.set
		
		JPEGImageEncoder enc = JPEGCodec.createJPEGEncoder(new FileOutputStream(new File("tag.jpg")));
		enc.encode(img1);
		
	}
}
