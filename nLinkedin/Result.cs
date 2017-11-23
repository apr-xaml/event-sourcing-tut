using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedin
{
    public class Result<TExplanation>
    {
        private readonly TExplanation _explanation;

        Result(bool isOk, TExplanation explanation)
        {
            IsOk = isOk;
            _explanation = explanation;
        }


        static public Result<TExplanation> Ok { get; } = new Result<TExplanation>(isOk: true, explanation: default);
        static public Result<TExplanation> Error(TExplanation expl) => new Result<TExplanation>(isOk: false, explanation: expl);

        public bool IsOk { get; }
        public TExplanation ExplanationIfNotOk
        {
            get
            {
                if(IsOk)
                {
                    throw new InvalidOperationException();
                }

                return _explanation;
            }

        }
    }
}
