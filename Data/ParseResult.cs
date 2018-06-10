using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM4DesignTool.Data
{
    public struct ParseResult
    {
        private bool parseSucces;

        private string rawText;

        public ParseResult(bool parseSucces = false, string rawText = "") : this()
        {
            this.ParseSucces = parseSucces;
            this.RawText = rawText;

        }

        public bool ParseSucces
        {
            get => this.parseSucces;
            set => this.parseSucces = value;
        }

        public string RawText
        {
            get => this.rawText;
            set => this.rawText = value;
        }
    }
}
