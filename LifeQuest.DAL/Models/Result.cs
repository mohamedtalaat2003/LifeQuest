using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.DAL.Models
{
    public class Result
    {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }

            public static Result SuccessResult()
            {
                return new Result { Success = true };
            }

            public static Result Failure(string error)
            {
                return new Result
                {
                    Success = false,
                    ErrorMessage = error
                };
            }
    }
}
