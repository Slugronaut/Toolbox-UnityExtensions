using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Toolbox
{
	public static class FileUtils
	{
		/// <summary>
		/// Depth-first recursive delete, with handling for descendant 
		/// directories open in Windows Explorer.
		/// </summary>
		public static void DeleteDirectory(string path)
		{
			foreach (string directory in Directory.GetDirectories(path))
			{
				DeleteDirectory(directory);
			}
			
			try
			{
				//Sleep(0);
				Directory.Delete(path, true);
			}
			catch (IOException) 
			{
				Directory.Delete(path, true);
			}
			catch (UnauthorizedAccessException)
			{
				Directory.Delete(path, true);
			}
		}

		/// <summary>
		/// Deletes the destination folder recursively with magic dust.
		/// </summary>
		/// <param name="destinationDir">Destination dir.</param>
		public static void DeleteRecursivelyWithMagicDust(string destinationDir) 
		{
			const int magicDust = 10;
			for (var gnomes = 1; gnomes <= magicDust; gnomes++) 
			{
				try 
				{
					Directory.Delete(destinationDir, true);
				} 
				catch (DirectoryNotFoundException) 
				{
					return;  // good!
				} 
				catch (IOException)
				{ 
                    // System.IO.IOException: The directory is not empty
					//Debug.Log("Gnomes prevent deletion of {0}! Applying magic dust, attempt #{1}.", destinationDir, gnomes);
					
					// see http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true for more magic
					Thread.Sleep(50);
					continue;
				}
				return;
			}
			// depending on your use case, consider throwing an exception here
		}
	}


}
