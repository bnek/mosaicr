import java.awt.Dimension;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;
import java.util.StringTokenizer;

import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import com.aetrion.flickr.FlickrException;
import com.aetrion.flickr.photos.Size;

import jpeg.JpegTools;
import fickr.FickrTools;


public class FlickrCollection {
	
	private FickrTools tools = null;
	Properties props = null;
	
	public FlickrCollection(String propertiesFileName) {
		props = readSettings(propertiesFileName);
		try {
			tools = new FickrTools(props);
		} catch (IOException e) {
			System.out.println("Error: "+e.getMessage());
			e.printStackTrace();
		} catch (ParserConfigurationException e) {
			System.out.println("Error: "+e.getMessage());
			e.printStackTrace();
		}
	}
	
	/**
	 * read sttings from props file.
	 * @param prpsFileName propsfile 
	 * @return
	 */
	private Properties readSettings(String prpsFileName) {
		System.out.println("readings settings from file '"+prpsFileName+"'");

        try {
            InputStream in = getClass().getResourceAsStream(prpsFileName);
            props = new Properties();
			props.load(in);
		} catch (IOException e) {
			System.err.println("couldnt load properties from file: "+prpsFileName);
			e.printStackTrace();
		}
		System.out.println("settings:\r\n"+props);
		return props;
	}


	public static void main(String[] args) {
		String propsFileName = ( (args != null && args[0]!= null) ? args[0] : "settings.properties");
		FlickrCollection col = new FlickrCollection(propsFileName);
		String [] argss = new String[8];
		for(int i=1; i<args.length; i++) {
			argss[i-1] = args[i];
		}
		try {
			col.createMosaic(argss[0], argss[1], argss[2], argss[3], argss[4], argss[5], argss[6], argss[7]);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private void createMosaic(String directoryWithJpegs, String mosaicFileName,
			String tileDimensionsX, String tileDimensionsY,
			String aspectRatioX, String aspectRatioY, String shuffleEnabled, String fillUpMissingTilesWithImages) throws IOException {
		if(directoryWithJpegs == null || !(new File(directoryWithJpegs).exists())) {
			new File(directoryWithJpegs).mkdirs();
		}
		if(mosaicFileName == null) {
			mosaicFileName = "mosaic.jpg";
		}
		if((new File(mosaicFileName)).exists()) {
			mosaicFileName = mosaicFileName.substring(0, mosaicFileName.length()-4).
				concat(String.valueOf(System.currentTimeMillis())).
				concat(".jpg");
		}
		
		if(props.containsKey("downloadSets")) {
			System.out.println("Downloading Sets from Flickr...");
			StringTokenizer tokenizer = new StringTokenizer(props.getProperty("downloadSets"), ",");
			if(tokenizer.countTokens() > 0) {
				List<String> sets = new ArrayList<String>(tokenizer.countTokens());
				while(tokenizer.hasMoreElements()) {
					sets.add(tokenizer.nextToken());
				}
				System.out.println("Sets: "+sets);
				try {
					int size = getFlickrSize(props);
					tools.downloadSets(new File(directoryWithJpegs), sets, size);
				} catch (SAXException e) {
					System.err.println("flickr set dowload failed...");
					e.printStackTrace();
				} catch (FlickrException e) {
					System.err.println("flickr set dowload failed...");
					e.printStackTrace();
				}
			}
			
		}
		
		// Tile dimensions
		int tileWidth = (tileDimensionsX == null) ? Integer.parseInt(props.getProperty("defaultTileWidth")) : Integer.parseInt(tileDimensionsX);
		int tileHeight = (tileDimensionsY == null) ? Integer.parseInt(props.getProperty("defaultTileHeight")) : Integer.parseInt(tileDimensionsY);
		Dimension tileDimensions = new Dimension(
				tileWidth,
				tileHeight
		);
		
		// aspect Ratio
		int aspectX = (aspectRatioX==null) ? Integer.parseInt(props.getProperty("defaultHorizontalTileCount")) : Integer.parseInt(aspectRatioX);
		int aspectY = (aspectRatioY==null) ? Integer.parseInt(props.getProperty("defaultVerticalTileCount")) : Integer.parseInt(aspectRatioY);
		Dimension aspectRatio = new Dimension(
				aspectX,
				aspectY
		);
		
		// iShuffleEnabled
		boolean iShuffleEnabled = (shuffleEnabled==null) ? Boolean.parseBoolean(props.getProperty("shuffleEnabled")) : Boolean.parseBoolean(shuffleEnabled);
		props.setProperty("shuffleEnabled", ""+iShuffleEnabled);
		
		// iFillUpMissingTilesWithImages
		boolean iFillUpMissingTilesWithImages = (fillUpMissingTilesWithImages==null) ? Boolean.parseBoolean(props.getProperty("fillUpMissingTilesWithImages")) : Boolean.parseBoolean(fillUpMissingTilesWithImages);
		props.setProperty("fillUpMissingTilesWithImages", ""+iFillUpMissingTilesWithImages);

		new JpegTools(props).createMosaicJpegFromDirectory(
				new File(directoryWithJpegs),
				new File(mosaicFileName),
				tileDimensions,
				aspectRatio
		);
	}

	private int getFlickrSize(Properties props2) {
		int size = Size.SQUARE;
		if(props.containsKey("flickrDownloadSize")) {
			String propSize = props.getProperty("flickrDownloadSize");
			System.out.println("Size is: "+propSize);
			//|MEDIUM|ORIGINAL|SMALL|SQUARE|THUMB
			if(propSize.equals("LARGE")) {
				size = Size.LARGE;
			}
			else if(propSize.equals("MEDIUM")) {
				size = Size.MEDIUM;
			}
			else if(propSize.equals("ORIGINAL")) {
				size = Size.ORIGINAL;
			}
			else if(propSize.equals("SMALL")) {
				size = Size.SMALL;
			}
			else if(propSize.equals("SQUARE")) {
				size = Size.SQUARE;
			}
			else if(propSize.equals("THUMB")) {
				size = Size.THUMB;
			}
		}
		return size;
	}

}
