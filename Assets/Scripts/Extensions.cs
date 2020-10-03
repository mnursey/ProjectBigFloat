using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

	public static float Mod(this float val, int mod){
		return ((val %= mod) < 0) ? mod+val : val;
	}
}

