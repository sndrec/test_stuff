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

	public static void PlayDesiredStage(int StageIndex)
	{
		MyGame GameEnt = Game.Current as MyGame;
		switch (StageIndex)
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
				st006.CreateStage();
				break;
			case 6:
				st007.CreateStage();
				break;
			case 7:
				st011.CreateStage();
				break;
			case 8:
				st012.CreateStage();
				break;
			case 9:
				st008.CreateStage();
				break;
			case 10:
				stw1bonus.CreateStage();
				break;
			case 11:
				st013.CreateStage();
				break;
			case 12:
				st010.CreateStage();
				break;
			case 13:
				st019.CreateStage();
				break;
			case 14:
				st015.CreateStage();
				break;
			case 15:
				st016.CreateStage();
				break;
			case 16:
				st017.CreateStage();
				break;
			case 17:
				st018.CreateStage();
				break;
			case 18:
				st014.CreateStage();
				break;
			case 19:
				st009.CreateStage();
				break;
			case 20:
				stw1bonus2.CreateStage();
				break;
			case 21:
				st020.CreateStage();
				break;
			case 22:
				st005.CreateStage();
				break;
			case 23:
				st021.CreateStage();
				break;
			case 24:
				staward.CreateStage();
				break;
			case 25:
				GameEnt.EndCourse();
				CurrentStage = 0;
				break;
			case 26:
				break;
		}
	}

	public static void PlayNextStage()
	{
		CurrentStage++;
		MyGame GameEnt = Game.Current as MyGame;
		GameEnt.StageInCourse = CurrentStage;
		PlayDesiredStage(CurrentStage);
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
				st021.CreateStage();
				break;
		}
	}
}

