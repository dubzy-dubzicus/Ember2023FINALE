using System.Text.Json.Serialization;

namespace asdfasdf;

[Serializable]
public class RoomDTO
{
	public long RoomId { get; set; }

	[JsonIgnore]
	public string Name
	{
		get { return this.RawFriendlyName ?? this.RawName; }
		set
		{
			if (this.RawFriendlyName == null)
			{
				this.RawName = value;
				return;
			}

			// QDCQCKQKNNM.YHFRKBSTWMR.XYVJTJVQPCA(this.RawName == this.RawFriendlyName, "Only expecting RawFriendlyName to be used when RawName acts as a non-user-visible, non-user-editable string that's different than the room's friendly display name. Therefore, this setter only writes name modifications back to RawFriendlyName. If this is firing, that assumption must be being broken or this setter is being used in the wrong use case. Please investigate.");
			this.RawFriendlyName = value;
		}
	}

	[JsonPropertyName("Name")]
	public string RawName
	{
		get;
		set;
	}
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyName("FriendlyName")]
	public string RawFriendlyName { get; set; }
	public string Description { get; set; }
	public string ImageName
	{
		get;
		set;
	}
	public Accessibility Accessibility { get; set; }
	public bool IsPublished
	{
		get
		{
			return this.Accessibility == Accessibility.Public;
		}
	}
	public bool SupportsScreens { get; set; }
	public bool SupportsWalkVR { get; set; }
	public bool SupportsTeleportVR { get; set; }
	public AgeRating AgeRating { get; set; }
	public WarningMask WarningMask { get; set; }
	public string CustomWarning { get; set; }
	public MaxPlayerCalculationMode MaxPlayerCalculationMode { get; set; }
	public DateTime CreatedAt { get; set; }
	public bool CloningAllowed { get; set; }
	public byte UgcVersion { get; set; }
	public bool IsRoomV2
	{
		get
		{
			return this.UgcVersion == 2;
		}
	}

	public bool IsDeveloperOwned { get; set; }
	public bool IsDorm { get; set; }
	public int BadgeVouchersRemaining { get; set; }
}

public enum Accessibility : byte
{
	Private,
	Public,
	Unlisted
}

public enum AgeRating : byte
{
	None,
	AllAges,
	Age13Plus
}

[Flags]
public enum WarningMask
{
	None = 0,
	Scary = 1,
	Mature = 2,
	FlashingLights = 4,
	IntenseMotion = 8,
	Violence = 16,
	Custom = 32,
	Reports = 64
}

public enum MaxPlayerCalculationMode : byte
{
	AllSubrooms,
	OnlyEntrySubrooms
}