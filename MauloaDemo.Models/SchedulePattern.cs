using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_pattern")]
    public class SchedulePattern : IValidatableObject {

        public SchedulePattern() {
            this.Lines = new List<SchedulePatternLine>();
            this.Items = new List<SchedulePatternItem>();
            this.Notes = new List<SchedulePatternNote>();
        }


        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sch_pattern_id { get; set; }

        [Required, StringLength(100)]
        public string description { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [ForeignKey("sch_pattern_id")]
        public virtual ICollection<SchedulePatternLine> Lines { get; set; }

        [ForeignKey("sch_pattern_id")]
        public virtual ICollection<SchedulePatternItem> Items { get; set; }

        [ForeignKey("sch_pattern_id")]
        public virtual ICollection<SchedulePatternNote> Notes { get; set; }         


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            return null;
        }

        public void SortLines() {
            if (this.Lines == null) return;

            var orderedLines = this.Lines.ToArray().OrderBy(m => m.min_offset).ThenBy(m => m.sch_pattern_line_id).ToList();
            this.Lines= orderedLines;
        }

        public void SortNotes() {
            if (this.Notes == null) return;

            var orderedNotes = this.Notes.ToArray().OrderBy(m => m.disp_seq).ThenBy(m => m.row_id).ToList();
            this.Notes = orderedNotes;
        }



    }
}
