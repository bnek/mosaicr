/**
 * 
 */
package jpeg.imagefiller;

import java.util.Random;

/**
 * The abstract class that defines base func. for image filling.
 * @author possum
 *
 */
public abstract class AbstractImageFillingStrategy implements ImageFillingStrategy {
	/** blur */
	private int placementBlurPixels = 0;
	/** random */
	private final Random random = new Random();
	
	protected int getBlurOffset() {
		if(placementBlurPixels==0) {
			return 0;
		}
		return random.nextInt(placementBlurPixels);
	}
	
	public int getPlacementBlurPixels() {
		return placementBlurPixels;
	}

	public void setPlacementBlurPixels(int placementBlurPixels) {
		this.placementBlurPixels = placementBlurPixels;
	}
}
