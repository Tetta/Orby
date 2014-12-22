using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

	/*
		stars = 0 - 3;
		--------------
		none = 0;
		time = 1 - 99;
		web = 100 - 199;
		sluggish = 201;
		destroyer = 202;
		yeti = 203;
		groot = 204;
	*/
public class staticClass {

	//public static staticClass instance = new staticClass();

	//public static List<level> levels = new List<level>();
	public static int[,] levels = new int[101, 2];

	public static void initLevels () {
		levels[1, 0] = 2;
		levels[1, 1] = 1;
		levels[2, 0] = 2;
		levels[2, 1] = 100;

	}

	public static int useWeb = 0;
	public static int timer = 0;
	public static bool useSluggish = false;
	public static bool useDestroyer = false;
	public static bool useYeti = false;
	public static bool useGroot = false;
}

