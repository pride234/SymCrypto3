using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace CryptoLab3
{
	class Program
	{

		static IOrderedEnumerable<KeyValuePair<char, int>>   OrderedLetters;
		static IOrderedEnumerable<KeyValuePair<string, int>> OrderedBigrams;

		static IOrderedEnumerable<KeyValuePair<string, int>> NoCrossing;
		static IOrderedEnumerable<KeyValuePair<string, int>> Crossing;

		static List<char> let = new List<char> {'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ы', 'э', 'ю', 'я'};
		static int m = let.Count;
		static string cipher;
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

		static void Main(string[] args)
		{
			
			GetBig();
			GetCipher();

			int ind = 0;

			for (int i = 0; i<5; i++) {

				for (int j = i+1; j<5; j++) {

					string[] some = new string[] { "ст", "но", "то", "на", "ен" };

					//int X1 = let.IndexOf(NoCrossing.ElementAt(i).Key[0]) * m + let.IndexOf(NoCrossing.ElementAt(i).Key[1]);
					//int X2 = let.IndexOf(NoCrossing.ElementAt(j).Key[0]) * m + let.IndexOf(NoCrossing.ElementAt(j).Key[1]);

					int Y1 = let.IndexOf(OrderedBigrams.ElementAt(i).Key[0]) * m + let.IndexOf(OrderedBigrams.ElementAt(i).Key[1]);
					int Y2 = let.IndexOf(OrderedBigrams.ElementAt(j).Key[0]) * m + let.IndexOf(OrderedBigrams.ElementAt(j).Key[1]);

					for (int k = 0; k < 5; k++) {

						for (int l = 0; l < 5; l++) {

							if (l == k) continue;

							//int X1 = let.IndexOf(some[k][0]) * m + let.IndexOf(some[k][1]);
							//int X2 = let.IndexOf(some[l][0]) * m + let.IndexOf(some[l][1]);

							int X1 = let.IndexOf(NoCrossing.ElementAt(k).Key[0]) * m + let.IndexOf(NoCrossing.ElementAt(k).Key[1]);
							int X2 = let.IndexOf(NoCrossing.ElementAt(l).Key[0]) * m + let.IndexOf(NoCrossing.ElementAt(l).Key[1]);

							//int Y1 = let.IndexOf(OrderedBigrams.ElementAt(k).Key[0]) * m + let.IndexOf(OrderedBigrams.ElementAt(k).Key[1]);
							//int Y2 = let.IndexOf(OrderedBigrams.ElementAt(l).Key[0]) * m + let.IndexOf(OrderedBigrams.ElementAt(l).Key[1]);

							int[] a = Linear(X1 - X2, Y1 - Y2, m*m);

							if (a == null) continue;

							HashSet<int> Keys = new HashSet<int> (a);

							foreach(int t in Keys) {
							//for (int t = 0; t<a.Length; t++) {

								int b = (Y1 - t*X1)%(m*m);

								if (b < 0) b = (m*m) + b;

								string plaintext = "";

								bool skip = false;

								for (int n = 0; n < cipher.Length - 1; n+=2) {

									int X = Reverse(t, m*m) * (let.IndexOf(cipher[n])*m + let.IndexOf(cipher[n+1]) - b);
									X = X%(m*m);
									if (X < 0) X = m*m + X;

									int x2 = X%m;
									int x1 = (X - x2)/m;
									plaintext += let[x1];
									plaintext += let[x2];
								}

								for (int n = 0; n<plaintext.Length - 1; n++) {

									if (plaintext.Substring(n, 2) == "ыы") skip = true;
									if (plaintext.Substring(n, 2) == "ьь") skip = true;
									if (plaintext.Substring(n, 2) == "аь") skip = true;
									if (plaintext.Substring(n, 2) == "що") skip = true;

								}

								if (skip == true) continue;

								ind++;

								Console.WriteLine("a = " + t + ", b = " + b + "\n" + plaintext + "\n");
							}
						}
					}
				} 
			}

			Console.WriteLine(ind);
			Console.ReadKey();
		}
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

		static int Reverse (int a, int n) {

			if (a < 0) a = n + a;

			int i = n, v = 0, d = 1;
			while (a > 0) {

				int t = i / a, x = a;
				a = i % x;
				i = x;
				x = d;
				d = v - t * x;
				v = x;
			}
			v %= n;
			if (v < 0) v = (v + n) % n;
			return v;
		}
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

		static int[] Linear (int a, int b, int n) {
			
			if (a < 0)
				a = n + a;
			if (b < 0)
				b = n + b;

			int d = gcd(a, n);

			if (d == 1) return new int[] { (b * Reverse(a, n)) % n };
			
			if (b%d != 0) return null;
			else {

				int[] res = new int[d];
				res [0] = ((b/d) * Reverse(a/d, n/d) % (n/d));
				int n1 = n/d;
				for (int i  = 1; i<res.Length; i++) res[i] = res[i-1] + n1;
				return res;
			}
		}
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

		static int gcd (int a, int b) {

			while (a != 0 && b != 0) {

				if (a > b) a %= b;
				else b %= a;
			}

			return a == 0 ? b : a;
		}
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

		static void GetBig() {

			StreamReader sr = new StreamReader(@"C:\Users\PRIDE\source\repos\CryptoLab3\text.txt");
			string text = sr.ReadToEnd();
			sr.Close();

			Dictionary<string, int> bigrams = new Dictionary<string, int>(1024);

			for (int i = 0; i < text.Length - 1; i += 2)
			{

				if (bigrams.ContainsKey(text.Substring(i, 2)) == false) bigrams.Add(text.Substring(i, 2), 0);

				bigrams[text.Substring(i, 2)] += 1; 
			}

			NoCrossing = from pair in bigrams
					 orderby pair.Value descending
					 select pair;

			bigrams = new Dictionary<string, int>(1024);

			for (int i = 0; i < text.Length - 1; i++)
			{

				if (bigrams.ContainsKey(text.Substring(i, 2)) == false) bigrams.Add(text.Substring(i, 2), 0);

				bigrams[text.Substring(i, 2)] += 1;
			}

			Crossing = from pair in bigrams
					   orderby pair.Value descending
					   select pair;
		}
//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

		static void GetCipher() {

			StreamReader sr = new StreamReader(@"C:\Users\PRIDE\source\repos\CryptoLab3\cipher_text.txt");
			cipher = sr.ReadToEnd();
			sr.Close();

			Regex rgx = new Regex("[^а-я]");
			cipher = rgx.Replace(cipher, "");

			Dictionary<char, int> Cipher_letters = new Dictionary<char, int>(m);
			Dictionary<string, int> Cipher_bigrams = new Dictionary<string, int>(m * m);

			for (int i = 0; i < cipher.Length; i++)
			{

				if (Cipher_letters.ContainsKey(cipher[i])) { Cipher_letters[cipher[i]] += 1; continue; }
				Cipher_letters.Add(cipher[i], 1);
			}

			OrderedLetters = from pair in Cipher_letters
								 orderby pair.Value descending
								 select pair;

			for (int i = 0; i < cipher.Length - 1; i += 2)
			{

				if (Cipher_bigrams.ContainsKey(cipher.Substring(i, 2)) == false) Cipher_bigrams.Add(cipher.Substring(i, 2), 0);

				Cipher_bigrams[cipher.Substring(i, 2)] += 1;
			}

			OrderedBigrams = from pair in Cipher_bigrams
								 orderby pair.Value descending
								 select pair;

			string write = "";

			foreach (KeyValuePair<string, int> a in OrderedBigrams) write += a.Key + " " + a.Value + "\n";

			StreamWriter wr = new StreamWriter(@"C:\Users\PRIDE\source\repos\CryptoLab3\freq.csv");

			wr.Write(write);
			wr.Close();
		}
	}
}
