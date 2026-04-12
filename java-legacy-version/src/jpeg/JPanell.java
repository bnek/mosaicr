package jpeg;

import java.awt.Color;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.image.BufferedImage;

import javax.swing.JPanel;

public class JPanell extends JPanel {

	private BufferedImage image;
	
	public BufferedImage getImage() {
		return image;
	}

	public JPanell(BufferedImage img) {
		super(true);
		image = img;
	}
	
	public Graphics getGraphics() {
		if(image != null) {
			return image.getGraphics();
		}
		else {return null;}
	}
	
	public void paint(Graphics g) {
		System.out.println("bla");
		if(image != null) {
			g.drawImage(image, 0, 0, this);
		}
		//super.paint(g);
	}
	public void repaint() {
		paint(getGraphics());
	}
}
