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
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.StageInCourse = CurrentStage;
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
				stw1bonus.CreateStage();
				break;
			case 6:
				st005.CreateStage();
				break;
			case 7:
				st006.CreateStage();
				break;
			case 8:
				st007.CreateStage();
				break;
			case 9:
				st008.CreateStage();
				break;
			case 10:
				st009.CreateStage();
				break;
			case 11:
				staward.CreateStage();
				break;
			case 12:
				GameEnt.EndCourse();
				CurrentStage = 0;
				break;
		}
	}
}

public class course_test
{
	public static int CurrentStage {get;set;}

	public static string CourseName = "Test Course";

	public static string CourseDescription = "A simple course, for beginner players to get their footing.";

	public static void PlayNextStage()
	{
		CurrentStage++;
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.StageInCourse = CurrentStage;
		switch (CurrentStage)
		{
			default:
				staward.CreateStage();
				break;
		}
	}
}

