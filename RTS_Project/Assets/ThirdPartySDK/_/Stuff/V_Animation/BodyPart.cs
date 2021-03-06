#region Info
// -----------------------------------------------------------------------
// BodyPart.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
namespace V_AnimationSystem
{

	public class BodyPart
	{






		public const int Head = 0;
		public const int Body = 1;
		public const int HandL = 2;
		public const int HandR = 3;
		public const int FootL = 4;
		public const int FootR = 5;
		public const int Sword = 6;
		public const int WeaponSecondary = 7;
		public const int Bow = 8;
		public const int Arrow = 9;
		public const int Shield = 10;
		public const int Trap = 11;
		public const int HairHat = 12;
		public const int StunRock = 13;

		public const int Pointer_1 = 14;
		public const int Pointer_2 = 15;
		public const int Pointer_3 = 16;

		public const int Weapon = 17;
		public const int Secondary = 18;



		public const int Custom = 99;
		public string customName;

		public int preset;

		public BodyPart(int preset)
		{
			this.preset = preset;
			customName = ((Old_BodyPart)preset).ToString();
		}
		public BodyPart(string customName)
		{
			this.customName = customName;
			preset = Custom;
		}
		public BodyPart(int preset, string customName)
		{
			this.preset = preset;
			this.customName = customName;
		}

		public bool Equals(BodyPart other)
		{
			return customName == other.customName;
		}

		public override string ToString()
		{
			return customName;
		}


		public static string Save_Static(BodyPart single)
		{
			return single.Save();
		}
		public string Save()
		{
			//Returns a string to be used in savefiles
			string[] content =
			{
				"" + preset,
				"" + customName
			};
			return string.Join("#BODYPART#", content);
		}

		public static BodyPart Load(string save)
		{
			if (!save.Contains("#BODYPART#"))
			{
				// OLD Enum based BodyPart
				var oldBodyPart
						= V_Animation.GetEnumFromString<Old_BodyPart>(save);
				return new BodyPart((int)oldBodyPart);
			}
			var content = V_Animation.SplitString(save, "#BODYPART#");

			var preset = V_Animation.Parse_Int(content[0]);
			V_Animation.StringArrPushIfIndex(1, ref content,
					((Old_BodyPart)preset).ToString());
			var customName = content[1];

			return new BodyPart(preset, customName);
		}
	}

	public enum Old_BodyPart
	{
		Head,
		Body,
		HandL,
		HandR,
		FootL,
		FootR,
		Sword,
		WeaponSecondary,
		Bow,
		Arrow,
		Shield,
		Trap,
		HairHat,
		StunRock,

		Pointer_1,
		Pointer_2,
		Pointer_3,

		Weapon,
		Secondary
	}

}
