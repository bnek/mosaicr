/**
 * 
 */
package jpeg.imagefiller;

import java.awt.Dimension;
import java.awt.Rectangle;
import java.awt.image.BufferedImage;

/**
 * @author possum
 *
 */
public class SimpleImageFillingStrategy extends AbstractImageFillingStrategy {

	/** compute the next part to be filled*/
	public Rectangle computeBoundsForNextTile(BufferedImage mosaic,
			Dimension tileDimensions, int xPos, int yPos) {
		int blurOffset = getBlurOffset();
		int x = xPos*tileDimensions.width+blurOffset;
		int dx = tileDimensions.width;
		if((x+dx) > mosaic.getWidth()) {
			x -= (x+dx)-mosaic.getWidth();
		}
		int y = yPos*tileDimensions.height+blurOffset;
		int dy = tileDimensions.height;
		if((y+dy) > mosaic.getHeight()) {
			y -= (y+dy)-mosaic.getHeight();
		}
		return new Rectangle(x, y, dx, dy);
	}
}
