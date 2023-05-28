using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuestionnaireProject.Attributes
{
    public class CorrectAnswerAttr:ValidationAttribute
    {       

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var correctAnswer = value as int?;
            int validCorrectAnswer = -1;

            var answersText = validationContext.ObjectType.GetProperty("AnswersText");
            if (answersText != null && correctAnswer!=null)
            {
                var answersTextValues = answersText.GetValue(validationContext.ObjectInstance, null);
                var castedAnswersTextValues = answersTextValues as List<string>;
                int counter = 1;
                if (castedAnswersTextValues != null)
                {
                    int answersCount = castedAnswersTextValues.Count();
                    for (int i = 0; i < answersCount; i++)
                    {
                        if (castedAnswersTextValues[i] != "")
                        {
                            if (correctAnswer - 1 == i)
                            {
                                validCorrectAnswer = counter;
                            }

                            counter++;
                        }
                    }
                }

                if (validCorrectAnswer == -1)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }

            }
            

            
            return null;
        }
       
    }
}