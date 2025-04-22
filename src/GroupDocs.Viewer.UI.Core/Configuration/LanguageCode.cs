namespace GroupDocs.Viewer.UI.Core.Configuration
{
    public class LanguageCode
    {
        public static readonly LanguageCode Arabic = new LanguageCode("ar");
        public static readonly LanguageCode Catalan = new LanguageCode("ca");
        public static readonly LanguageCode Czech = new LanguageCode("cs");
        public static readonly LanguageCode Croatian = new LanguageCode("hr");
        public static readonly LanguageCode Danish = new LanguageCode("da");
        public static readonly LanguageCode German = new LanguageCode("de");
        public static readonly LanguageCode Greek = new LanguageCode("el");
        public static readonly LanguageCode English = new LanguageCode("en");
        public static readonly LanguageCode Spanish = new LanguageCode("es");
        public static readonly LanguageCode Filipino = new LanguageCode("fil");
        public static readonly LanguageCode French = new LanguageCode("fr");
        public static readonly LanguageCode Hebrew = new LanguageCode("he");
        public static readonly LanguageCode Hindi = new LanguageCode("hi");
        public static readonly LanguageCode Indonesian = new LanguageCode("id");
        public static readonly LanguageCode Italian = new LanguageCode("it");
        public static readonly LanguageCode Japanese = new LanguageCode("ja");
        public static readonly LanguageCode Kazakh = new LanguageCode("kk");
        public static readonly LanguageCode Korean = new LanguageCode("ko");
        public static readonly LanguageCode Malay = new LanguageCode("ms");
        public static readonly LanguageCode Dutch = new LanguageCode("nl");
        public static readonly LanguageCode Polish = new LanguageCode("pl");
        public static readonly LanguageCode Portuguese = new LanguageCode("pt");
        public static readonly LanguageCode Romanian = new LanguageCode("ro");
        public static readonly LanguageCode Russian = new LanguageCode("ru");
        public static readonly LanguageCode Swedish = new LanguageCode("sv");
        public static readonly LanguageCode Vietnamese = new LanguageCode("vi");
        public static readonly LanguageCode Thai = new LanguageCode("th");
        public static readonly LanguageCode Turkish = new LanguageCode("tr");
        public static readonly LanguageCode Ukrainian = new LanguageCode("uk");
        public static readonly LanguageCode ChineseSimplified = new LanguageCode("zh-hans");
        public static readonly LanguageCode ChineseTraditional = new LanguageCode("zh-hant");

        public string Value { get; }

        public static LanguageCode[] All = new LanguageCode[]
        {
            Arabic,
            Catalan,
            Czech,
            Croatian,
            Danish,
            German,
            Greek,
            English,
            Spanish,
            Filipino,
            French,
            Hebrew,
            Hindi,
            Indonesian,
            Italian,
            Japanese,
            Kazakh,
            Korean,
            Malay,
            Dutch,
            Polish,
            Portuguese,
            Romanian,
            Russian,
            Swedish,
            Vietnamese,
            Thai,
            Turkish,
            Ukrainian,
            ChineseSimplified,
            ChineseTraditional
        };

        public LanguageCode(string value)
        {
            Value = value;
        }
    }
}