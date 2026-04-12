/**
 * 
 */
package jpeg;

import java.awt.Color;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Random;

/**
 * @author bnek
 *
 */
public class ColorRangeContainer implements Iterator<Color> {

	private List<ColorRange> colorRanges;
	private final Random random;
	private Iterator<ColorRange> it;
	
	public ColorRangeContainer(final Random random) {
		this.random = random;
		colorRanges = new ArrayList<ColorRange>();
	}
	
	public boolean add(ColorRange range) {
		return colorRanges.add(range);
	}
	
	public boolean remove(ColorRange range) {
		return colorRanges.add(range);
	}
	
	@Override
	public boolean hasNext() {
		return colorRanges.size() > 0;
	}

	@Override
	public Color next() {
		int nextIndex = random.nextInt(colorRanges.size());
		return colorRanges.get(nextIndex).getNextColor();
	}

	@Override
	public void remove() {
		
	}

}
