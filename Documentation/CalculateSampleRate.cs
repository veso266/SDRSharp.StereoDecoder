using System;
					
public class Program
{
	public static void Main()
	{
		int sampleRate = 300000; //Input sample rate
		int length = 14848; //Input Buffer Length
		int DecimationRatio = 3; //(int)Math.Round(Math.Sqrt(300000/37500));
		int _audioDecimationFactor = (int)Math.Pow(2.0, DecimationRatio); //Input sample rate / Output sample rate -> 300000 / 37500 = 8
		int AudioLength = length / _audioDecimationFactor;
		Console.WriteLine("length = {0}",length);
		Console.WriteLine("DecimationRatio = {0}",DecimationRatio);
		Console.WriteLine("_audioDecimationFactor = {0}",_audioDecimationFactor);
		Console.WriteLine("AudioLength = {0}", AudioLength); //Output buffer Length
		Console.WriteLine("Output sample rate = {0}", sampleRate / _audioDecimationFactor);
	}
}