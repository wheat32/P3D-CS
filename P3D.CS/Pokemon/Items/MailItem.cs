namespace P3D.Items;

public abstract class MailItem : Item
{
    public struct MailData
    {
        public String MailID;
        public String MailHeader;
        public String MailText;
        public String MailSender;
        public int MailAttachment;
        public String MailSignature;
        public String MailOriginalTrainerOT;
        public bool MailRead;
    }

    public override bool CanBeUsedInBattle { get; } = false;
    public override ItemTypes ItemType { get; } = ItemTypes.Mail;
    public override int PokeDollarPrice { get; protected set; } = 50;

    public static MailData GetMailDataFromString(String s)
    {
        s = s.Replace("|", @"\,");
        String[] data = s.Split(@"\,");
        return new MailData
        {
            MailID = data[0],
            MailSender = data[1],
            MailHeader = data[2],
            MailText = data[3],
            MailSignature = data[4],
            MailAttachment = int.Parse(data[5]),
            MailOriginalTrainerOT = data[6],
            MailRead = bool.Parse(data[7]),
        };
    }

    public static String GetStringFromMail(MailData mail)
    {
        return $@"{mail.MailID}\,{mail.MailSender}\,{mail.MailHeader}\,{mail.MailText}\,{mail.MailSignature}\,{mail.MailAttachment}\,{mail.MailOriginalTrainerOT}\,{mail.MailRead.ToNumberString()}";
    }
}
