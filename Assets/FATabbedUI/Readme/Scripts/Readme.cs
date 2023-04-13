using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Readme", menuName = "ReadMe")]
public class Readme : ScriptableObject {
	public Texture2D icon;
	public float iconMaxWidth = 128f;
	public string title;
	public Section[] sections;
	public bool loadedLayout;
	
	[Serializable]
	public class Section {
		public string heading, linkText, url;
		public string[] lines;
	}
}
