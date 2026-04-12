package jpeg;

import java.awt.image.BufferedImage;

/**
 * @author possum
 *
 */
public class MosaicImage {
	private BufferedImage image = null;
	
	public MosaicImage() {
		
	}
	
	public BufferedImage getImage() {
		return image;
	}

	public void setImage(BufferedImage image) {
		this.image = image;
	}
}
