using Sandbox;
using SandboxEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Sandbox;

//As stated elsewhere in my code...
//This is dumb. I just don't know another way to do it.
//"If it works, it works"

public class course_w1
{
	public static int CurrentStage {get;set;}

	public static string CourseName = "Basic Course";

	public static string CourseDescription = "A simple course, for beginner players to get their footing.";

	public static void PlayNextStage()
	{
		CurrentStage++;
		switch (CurrentStage)
		{
			case 1:
				st001.CreateStage();
				break;
			case 2:
				st002.CreateStage();
				break;
			case 3:
				st003.CreateStage();
				break;
			case 4:
				st004.CreateStage();
				break;
			case 5:
				st005.CreateStage();
				break;
			case 6:
				st006.CreateStage();
				break;
			case 7:
				MyGame GameEnt = Game.Current as MyGame;
				GameEnt.EndCourse();
				CurrentStage = 0;
				break;
		}
	}
}
