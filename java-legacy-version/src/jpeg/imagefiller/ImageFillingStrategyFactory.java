/**
 * 
 */
package jpeg.imagefiller;

import java.util.Properties;

/** @author possum */
public final class ImageFillingStrategyFactory {
	public static final String IMAGE_FILLING_STRATEGY = "imageFillingType";
	public static final String CLASS_NAME_SUFFIX = "_CLASS";
	public static final ImageFillingStrategy getInstance(Properties props) throws ClassNotFoundException, InstantiationException, IllegalAccessException {
		String instanceClassName = props.getProperty(props.getProperty(IMAGE_FILLING_STRATEGY).concat(CLASS_NAME_SUFFIX));
		return (ImageFillingStrategy) Class.forName(instanceClassName).newInstance();
	}
}
