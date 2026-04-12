package fickr;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import javax.xml.parsers.ParserConfigurationException;

import org.xml.sax.SAXException;

import com.aetrion.flickr.Flickr;
import com.aetrion.flickr.FlickrException;
import com.aetrion.flickr.REST;
import com.aetrion.flickr.RequestContext;
import com.aetrion.flickr.auth.Auth;
import com.aetrion.flickr.auth.Permission;
import com.aetrion.flickr.photos.Photo;
import com.aetrion.flickr.photos.PhotoList;
import com.aetrion.flickr.photos.PhotosInterface;
import com.aetrion.flickr.photos.Size;
import com.aetrion.flickr.photosets.Photoset;
import com.aetrion.flickr.photosets.PhotosetsInterface;
import com.aetrion.flickr.util.AuthStore;
import com.aetrion.flickr.util.FileAuthStore;

public class FickrTools {
	
    private String nsid = null;
    private Flickr flickr = null;
    private AuthStore authStore = null;
    
    public FickrTools(Properties props) 
      throws IOException, ParserConfigurationException {
        this.flickr = new Flickr(props.getProperty("apiKey"),
        		props.getProperty("secret"),
        		new REST()
        );
        this.nsid = props.getProperty("user");

        if (props.getProperty("authStoreDir") != null) {
            this.authStore = new FileAuthStore(new File(props.getProperty("authStoreDir")));
        }
    }

	private void authorize() throws IOException, SAXException, FlickrException {
		String frob = this.flickr.getAuthInterface().getFrob();

		URL authUrl = this.flickr.getAuthInterface().buildAuthenticationUrl(Permission.READ, frob);
		System.out.println("Please visit: " + authUrl.toExternalForm() + " then, hit enter.");

		System.in.read();

		Auth token = this.flickr.getAuthInterface().getToken(frob);
		RequestContext.getRequestContext().setAuth(token);
		this.authStore.store(token);
		System.out.println("Thanks.  You probably will not have to do this every time.  Now starting backup.");
	}
	
	public void downloadSet(File directory, String setName, int photoQuality) throws IOException, SAXException, FlickrException {
		List<String> list = new ArrayList<String>(1);
		list.add(setName);
		this.downloadSets(directory, list, photoQuality);
	}
	
	public void downloadSets(File directory, List<String> setNames, int photoQuality) throws IOException, SAXException, FlickrException {

		if (!directory.exists()) {
			directory.mkdir();
		}

		RequestContext rc = RequestContext.getRequestContext();
		
		
		if (this.authStore != null) {
			System.out.println();
			Auth auth = this.authStore.retrieve(this.nsid);
			if (auth == null) this.authorize();
			else rc.setAuth(auth);
		}


		PhotosetsInterface pi = flickr.getPhotosetsInterface();
		PhotosInterface photoInt = flickr.getPhotosInterface();
		Map allPhotos = new HashMap();

		Iterator sets = pi.getList(this.nsid).getPhotosets().iterator();

		while (sets.hasNext()) {
			Photoset set = (Photoset)sets.next();
			if(setNames.contains(set.getTitle())) {
				PhotoList photos = pi.getPhotos(set.getId(), 500, 1);
				allPhotos.put(set.getTitle(), photos);
			}
		}



		Iterator allIter = allPhotos.keySet().iterator();

		while (allIter.hasNext()) {
			int filesWritten = 0;
			String setTitle = (String) allIter.next();
			String setDirectoryName = makeSafeFilename(setTitle);

			Collection currentSet = (Collection) allPhotos.get(setTitle);
			Iterator setIterator = currentSet.iterator();
			while (setIterator.hasNext()) {

				Photo p = (Photo) setIterator.next();
				String url = p.getLargeUrl();
				URL u = new URL(url);
				String filename = u.getFile();
				filename = filename.substring(filename.lastIndexOf("/") + 1 , filename.length());
				System.out.print("Now writing " + filename + " to " + directory.getCanonicalPath());
				System.out.println(", "+(++filesWritten)+" of "+currentSet.size());
				BufferedInputStream inStream = new BufferedInputStream(photoInt.getImageAsStream(p, photoQuality));
				File newFile = new File(directory, filename);

				FileOutputStream fos = new FileOutputStream(newFile);

				int bytesInBuf;
				byte [] buf = new byte[1024];

				while ((bytesInBuf = inStream.read(buf)) != -1) {
					fos.write(buf, 0, bytesInBuf);
				}
				fos.flush();
				fos.close();
				inStream.close();
			}
		}
		System.out.println("done.");
	
	}

	public void doBackup(File directory) throws Exception {
		if (!directory.exists()) directory.mkdir();

		RequestContext rc = RequestContext.getRequestContext();
		
		
		if (this.authStore != null) {
			System.out.println();
			Auth auth = this.authStore.retrieve(this.nsid);
			if (auth == null) this.authorize();
			else rc.setAuth(auth);
		}


		PhotosetsInterface pi = flickr.getPhotosetsInterface();
		PhotosInterface photoInt = flickr.getPhotosInterface();
		Map allPhotos = new HashMap();

		Iterator sets = pi.getList(this.nsid).getPhotosets().iterator();

		while (sets.hasNext()) {
			Photoset set = (Photoset)sets.next();
			PhotoList photos = pi.getPhotos(set.getId(), 500, 1);
			allPhotos.put(set.getTitle(), photos);
		}

		int notInSetPage = 1;
		Collection notInASet = new ArrayList();
		while (true) {
			Collection nis = photoInt.getNotInSet(50, notInSetPage);
			notInASet.addAll(nis);
			if (nis.size() < 50) break;
			notInSetPage++;
		}
		allPhotos.put("NotInASet", notInASet);



		Iterator allIter = allPhotos.keySet().iterator();

		while (allIter.hasNext()) {
			String setTitle = (String) allIter.next();
			String setDirectoryName = makeSafeFilename(setTitle);

			Collection currentSet = (Collection) allPhotos.get(setTitle);
			Iterator setIterator = currentSet.iterator();
			File setDirectory = new File(directory, setDirectoryName);
			setDirectory.mkdir();
			while (setIterator.hasNext()) {
				Photo p = (Photo) setIterator.next();
				String url = p.getLargeUrl();
				URL u = new URL(url);
				String filename = u.getFile();
				filename = filename.substring(filename.lastIndexOf("/") + 1 , filename.length());
				System.out.println("Now writing " + filename + " to " + setDirectory.getCanonicalPath());
				BufferedInputStream inStream = new BufferedInputStream(photoInt.getImageAsStream(p, Size.SQUARE));
				File newFile = new File(setDirectory, filename);

				FileOutputStream fos = new FileOutputStream(newFile);

				int bytesInBuf;
				byte [] buf = new byte[1024];

				while ((bytesInBuf = inStream.read(buf)) != -1) {
					fos.write(buf, 0, bytesInBuf);
				}
				fos.flush();
				fos.close();
				inStream.close();
			}
		}

	}

	private String makeSafeFilename(String input) {
		byte[] fname = input.getBytes();
		byte[] bad = new byte[]{'\\', '/'};
		byte replace = '_';
		for (int i = 0; i < fname.length; i++) {
			for (int j = 0; j < bad.length; j++) {
				if (fname[i] == bad[j]) fname[i] = replace;
			}
		}
		return new String(fname);
	}

}
