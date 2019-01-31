namespace openstig_msg_score.Models
{
    public class Score
    {
        public int totalCat1Open { get; set; }
        public int totalCat1NotApplicable { get; set; }
        public int totalCat1NotAFinding { get; set; }
        public int totalCat1NotReviewed { get; set; }
        public int totalCat2Open { get; set; }
        public int totalCat2NotApplicable{ get; set; }
        public int totalCat2NotAFinding { get; set; }
        public int totalCat2NotReviewed { get; set; }
        public int totalCat3Open { get; set; }
        public int totalCat3NotApplicable { get; set; }
        public int totalCat3NotAFinding { get; set; }
        public int totalCat3NotReviewed { get; set; }

        public int totalOpen { get { return totalCat1Open + totalCat2Open + totalCat3Open;} }
        public int totalNotApplicable { get { return totalCat1NotApplicable + totalCat2NotApplicable + totalCat3NotApplicable;} }
        public int totalNotAFinding { get { return totalCat1NotAFinding + totalCat2NotAFinding + totalCat3NotAFinding;} }
        public int totalNotReviewed { get { return totalCat1NotReviewed + totalCat2NotReviewed + totalCat3NotReviewed;} }
    }
}