namespace asdfasdf;

[Serializable]
public class Outfit
{
    public int Accessibility { get; set; }
    public string CustomizationSettings { get; set; }
    public int DataVersion { get; set; }
    public OutfitLegacyData LegacyData { get; set; }
    public string Name { get; set; }
    public Array Selections { get; set; }
    public int Slot { get; set; }
    public string ThumbnailFileName { get; set; }
}

[Serializable]
public class OutfitLegacyData
{
    public string FaceFeatures { get; set; }
    public string HairColor { get; set; }
    public string SelectionsV1 { get; set; }
    public string SelectionsV2 { get; set; }
    public string SkinColor { get; set; }
}

[Serializable]
public class OutfitSaveRequest
{
    public int Slot { get; set; }
    public string PreviewImageName { get; set; }
    public string OutfitSelections { get; set; }
    public string OutfitSelectionsV2 { get; set; }
    public string FaceFeatures { get; set; }
    public string SkinColor { get; set; }
    public string HairColor { get; set; }
    public object[] CustomAvatarItems { get; set; }
}