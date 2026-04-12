package jpeg.imagefiller;

import java.awt.Dimension;
import java.awt.Rectangle;
import java.awt.image.BufferedImage;
import java.util.Random;

public class CircleImageFillingStrategy extends AbstractImageFillingStrategy {

	private Random rand = new Random();
	private BufferedImage mosaicRef;
	
	@Override
	public Rectangle computeBoundsForNextTile(BufferedImage mosaic,
			Dimension tileDimensions, int xPos, int yPos) {
		int x ;
		if(xPos <= 0) {
			x = 0;
		}
		else {
			x = getRandom(mosaic).nextInt(xPos * tileDimensions.width);
		}
		
		int y;
		if(yPos <= 0) {
			y = 0;
		}
		else {
			y = getRandom(mosaic).nextInt(yPos * tileDimensions.height);
		}
		
		int dx;
		if(x+x+tileDimensions.width > mosaic.getWidth()) {
			dx = tileDimensions.width;
		}
		else {
			dx = tileDimensions.width+x;
		}
		
		int dy;
		if(y+y+tileDimensions.height > mosaic.getHeight()) {
			dy = tileDimensions.height;
		}
		else {
			dy = tileDimensions.height+y;
		}
		return new Rectangle(x, y, dx, dy);
	}

	private Random getRandom(BufferedImage mosaic) {
		if(mosaicRef != mosaic) {
			rand = new Random();
			mosaicRef = mosaic;
		}
		return rand;
	}
}
