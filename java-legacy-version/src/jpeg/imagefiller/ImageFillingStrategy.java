package jpeg.imagefiller;

import java.awt.Dimension;
import java.awt.Rectangle;
import java.awt.image.BufferedImage;

public interface ImageFillingStrategy {
	public void setPlacementBlurPixels(int placementBlurPixels);
	public int getPlacementBlurPixels();
	public Rectangle computeBoundsForNextTile(BufferedImage mosaic,
			Dimension tileDimensions, int pos, int pos2);
}
