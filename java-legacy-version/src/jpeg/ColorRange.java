package jpeg;

import java.awt.Color;
import java.util.Random;

public class ColorRange {
	private Color lower;
	private Color upper;
	private final Random random;
	
	public ColorRange(Random r, Color lower, Color upper) {
		this.random = r;
		this.lower = lower;
		this.upper = upper;
	}
	
	public int getRange() {
		int range = 0;
		range += Math.abs( upper.getRed() - lower.getRed() );
		range += Math.abs( upper.getGreen() - lower.getGreen() );
		range += Math.abs( upper.getBlue() - lower.getBlue() );
		return range;
	}
	
	public Color getNextColor() {
		if(upper.equals(lower)){
			return lower;
		}
		// rgb ranges
		int rRange = Math.abs( upper.getRed() - lower.getRed() );
		int gRange = Math.abs( upper.getGreen() - lower.getGreen() );
		int bRange = Math.abs( upper.getBlue() - lower.getBlue() );
		// rgb values
		int r = rRange>0 ? lower.getRed()+random.nextInt(rRange) : lower.getRed();
		int g = gRange>0 ? lower.getGreen()+random.nextInt(gRange) : lower.getGreen();
		int b = bRange>0 ? lower.getBlue()+random.nextInt(bRange) : lower.getBlue();
		return new Color(r,g,b);
	}
	
	
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result + ((lower == null) ? 0 : lower.hashCode());
		result = prime * result + ((upper == null) ? 0 : upper.hashCode());
		return result;
	}

	
	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		ColorRange other = (ColorRange) obj;
		if (lower == null) {
			if (other.lower != null)
				return false;
		} else if (!lower.equals(other.lower))
			return false;
		if (upper == null) {
			if (other.upper != null)
				return false;
		} else if (!upper.equals(other.upper))
			return false;
		return true;
	}
	
}
