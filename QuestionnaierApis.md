# ENUMS

```c#
 public enum QuestionType
        {
            BenchMarking = 0,
            Validation = 1,
            Main = 2,
            Anatomical = 3,
            MainWithPicture = 4,
            SiteBenchMarking = 5
        }
        
public enum NotificationType
        {
            Danger = 0,
            Success = 1,
            Info = 2,
            Primary = 3,
            Warning = 4
        }
        
public enum Special
        {
            Ladder = 0,
            Bold = 1,
            Color = 2,
            ExtraDays = 3
        }
        
public enum Sex
        {
            FeMale = 0,
            Male = 1,
            All = 2
        }
        

```



# Login

URL

```c#
http://.../account/loginandroid
```

Input

```json
{
 	"NationalId": "1234567890",
    "Password": "*******",
    "RememberMe": "true" //false
}

```



Output

```json
{
    "result": true //false
}
```

# LogOff

URL:

```c#
http://.../account/loginandroid
```

Output: 

```json
{ result = true, message = "خروج با موفقیت صورت گرفت." }
```



# Register

URL

```c#
http://.../account/RegisterAndroid
```

Input

```json
{
    "NationalId":"...",
    "FirstName":"...",
    "LastName":"...",
    "Email": "...",
    "Password":"...",
    "Sex":"...",
    "StudyFieldId":"...",
    "CityId":"...",
    "BirthDate":"...",
    "PhoneNumber":"..."  
}
```

Output

```
{
    "result": true
}
```



# Forget Password

URL

```c#
http://.../account/ّForgotPasswordAndroid
```

Input:

```json
{
    "NationalId":"..."
}
```

Output:

```json
{
    result: true,
    message: "ایمیل بازیابی رمز برای شما ارسال گردید.",
    code ==> این کد بعدا حذف میشه فقط برای تایید اس ام اس هست الان. 
}
```

# User Profile Changes

اینو بعدا بهت میدم استاااد.

# Is Phone Confirmed?

اینو بعدا بهت میدم استاااد.

# Change Phone Number

با این اگه کانفیرم نکرده بود راهشو ببند دهنشو سرویس کن :دی

URL

```c#
http://.../account/IsPhoneNumberConfirmed
```

Input:

```json

```

Output:

```json
{
    "result": true,
}
```

# ConfirmPhoneNumber

حتما بعد از تغییر شماره یا ثبت نام باید کد رو تایید کنیاااا...

URL

```c#
http://.../account/ConfirmPhoneNumberAndroid
```

Input:

```json
{
    "code":"..."
}
```

Output:

```json
{
    "result": true,    
}
```



# Change Password

URL

```c#
http://.../manage/ChangePasswordAndroid
```

Input:

```json
{
    "OldPassword":"...",
    "NewPassword": "...",
    "ConfirmPassword":"..."
}
```

Output:

```json
{ result = true, message = "رمز عبور با موفقیت تغییر نمود." }
```



# Reset Password

URL

```c#
http://.../account/ResetPasswordAndroid
```

Input:

```json
{
    "NationalId":"...",
    "Password": "newPassword",
    "Code":"smsCode"
}
```

Output:

```json
{ result = true, message = "رمز عبور با موفقیت تغییر نمود." }
```



# Questionnaires

URL:

```c#
http://.../api/android/questionnaires
```

Output:

```json
{
    "questionnaire": [
        {
            "name": "پرسشنامه جدید",
            "id": "15",
            "specialId": 10,
            "dateExpire": "2018-12-19T21:22:06.68",
            "answeredByPerson": 0,
            "rate": 0,
            "eachPesronMoney": 90000,
            "benchMarkQuestionsAmount": 4,
            "otherQuestionsAmount": 5
        },
        {
            "name": "پرسشنامه بعدی",
            "id": "16",
            "specialId": 10,
            "dateExpire": null,
            "answeredByPerson": 0,
            "rate": 0,
            "eachPesronMoney": null,
            "benchMarkQuestionsAmount": 0,
            "otherQuestionsAmount": 0
        },
```

# Bench Marking Questions (Receiver)

This method returns the list of Bench marking questions.

URL:

```c#
http://.../api/android/benchmark
```

Input:

```json
{
    "Id": "..." //questionnaier id is needed here
}
```

Output:

```json
{
    "questions": [
        {
            "QuestionId": "1",
            "QuestionText": "لورم  سادگی نامفهوم از صنعت چاپ و با استفاد",
            "Answers": [
                {
                    "Id": "1108",
                    "Text": "لورم ایپسوم متن ساختگی با تولی و با استفاد"
                },
                {
                    "Id": "1109",
                    "Text": " سادگی نامفهوم از صنعت چاپ و با استفاد"
                },
                {
                    "Id": "1110",
                    "Text": "لورم ایپسوم متن ساختگی با تولید  و با استفاد"
                },
                {
                    "Id": "1111",
                    "Text": "ا تولید سادگی نامفهوم از صنعت چاپ و با استفاد"
                },
                {
                    "Id": "1112",
                    "Text": "لورم ایپسوم متن ساختگی با  و با استفاد"
                }
            ]
        },
        ...
```



# Bench Marking Questions (Sender)

It receives a list of answers and questions Id + the questionnaire Id

URL:

```c#
http://.../api/android/BenchMarkEvaluation
```

Input:

```json
{
"QuestionnaireId":"15",
"Responses": [
   		{
    		"QuestionId":"...",
    		"Answer":"..."
		}
	]
}
```

Output:

```json
{ 
    result = 1, 
    message = "لیاقت سنجی بدون غلط و با موفقیت پاسخ داده شد." 
}
```

‍Or:

```json
{
    result = 0, 
    message = "لیاقت سنجی غلط پاسخ داده شد." 
}
```

Or:

```json
{ 
    result = 0, 
    message = "خطایی رخ داده است." 
}
```

Or:

```json
{ 
    result = 0     
}
```

Or:

```json
{ 
    result = 0, 
    message = "شما صلاحیت پاسخگویی به پرسشنامه را ندارید."     
}
```



# Main Questions (Receiver)

URL:

```c#
http://.../api/android/Main
```

Input:

```json
{
    "Id": "..." //questionnaier id is needed here
}
```

Output:

```json
{
    "questions": [
        {
            "Questionnaire_Id": "15",
            "Id": "5",
            "Text": "تفاده از طراحان گرافیک است. ",  //Question Text
            "QuestionType": 2,
            "MultipleChoiceAnswers": [
                {
                    "Id": "212",
                    "Answer": " و با استفاده از طراحان گرافیک است. "
                },
                {
                    "Id": "213",
                    "Answer": " و با استفاده از طراحان گرافیک است. "
                },
                {
                    "Id": "214",
                    "Answer": "چاپ و با استفاده از طراحان گرافیک است. "
                },
                {
                    "Id": "215",
                    "Answer": "چاپ و با استفاده از طراحان گرافیک است. "
                }
            ],
            "VBAnswers": [],
            "Medias": [],
            "MediaType": 0
        },
        {
            "Questionnaire_Id": "15",
            "Id": "6",
            "Text": "فاده از طراحان گرافیک است. ",
            "QuestionType": 3,
            "MultipleChoiceAnswers": [],
            "VBAnswers": [],
            "Medias": [
                {
                    "Url": "76810a2c0af24716885905ccc915b1e8.jpg"
                }
            ],
            "MediaType": 1
        },
        {
            "Questionnaire_Id": "15",
            "Id": "7",
            "Text": "استفاده از طراحان گرافیک است. 1111",
            "QuestionType": 1,
            "MultipleChoiceAnswers": [],
            "VBAnswers": [
                {
                    "Id": "1125",
                    "Answer": "چاپ و با استفاده از طراحان گرافیک است. "
                },
                {
                    "Id": "1126",
                    "Answer": "چاپ و با استفاده از طراحان گرافیک است. "
                },
                {
                    "Id": "1127",
                    "Answer": "چاپ و با استفاده از طراحان گرافیک است. "
                },
                {
                    "Id": "1128",
                    "Answer": "چاپ و با استفاده از طراحان گرافیک است. "
                }
            ],
            "Medias": [],
            "MediaType": 0
        },
        {
            "Questionnaire_Id": "15",
            "Id": "8",
            "Text": "پرسش اول",
            "QuestionType": 2,
            "MultipleChoiceAnswers": [
                {
                    "Id": "216",
                    "Answer": "پاسخ 1"
                },
                {
                    "Id": "217",
                    "Answer": "پاسخ 2"
                },
                {
                    "Id": "218",
                    "Answer": "پاسخ3"
                },
                {
                    "Id": "219",
                    "Answer": "پاسخ 4"
                }
            ],
            "VBAnswers": [],
            "Medias": [
                {
                    "Url": "TJvWY"
                }
            ],
            "MediaType": 2
        },
        {
            "Questionnaire_Id": "15",
            "Id": "9",
            "Text": "راستی؟!",
            "QuestionType": 1,
            "MultipleChoiceAnswers": [],
            "VBAnswers": [
                {
                    "Id": "1121",
                    "Answer": "1"
                },
                {
                    "Id": "1122",
                    "Answer": "2"
                },
                {
                    "Id": "1123",
                    "Answer": "3"
                },
                {
                    "Id": "1124",
                    "Answer": "4"
                }
            ],
            "Medias": [],
            "MediaType": 0
        }
    ]
}
```

# Main Questions (Sender)

URL:

```c#
http://.../api/android/MainEvaluation
```

Input:

```json
{
"QuestionnaireId":"15",
"Responses": [
   		{
    		"QuestionId":"...",
    		"Answer":"..."
		}
	]
}
```

Output:

```json
{ 
    result = true, 
    message="پرسشنامه با موفقیت ذخیره شد." 
}
```

# Rate

URL:

```c#
http://.../api/android/Rate
```

Input:

```json
{
    Id:"...",
    "Rate":"..."
}
```

Output:

```json
{ 
    result = true, 
    message="پرسشنامه با موفقیت ذخیره شد." 
}
```

# Payments

URL:

```c#
http://.../api/android/Payments
```

Input:

```json
-
```

Output:

```json

{
    "Payments": [
        {
            "QuestionnaireName": "new",
            "DatePaid": "2018-12-18T00:42:28.003",
            "QuestionnairePrice": 1400,
            "DateFinished": "2018-12-18T00:42:28.003"
        },
        {
            "QuestionnaireName": "new",
            "DatePaid": "2018-12-18T00:42:28.003",
            "QuestionnairePrice": 1400,
            "DateFinished": "2018-12-18T00:42:28.003"
        }
    ]
}
```

# Send Payment Request

URL:

```c#
http://.../api/android/SendPaymentRequest
```

Input:

```json
{
    "cardNumber":"..."
}
```

Output:

```json
{
    result=Ok(), 
	message="درخواست با موفقیت ثبت گردید."
}
```

# Money Balance

URL:

```c#
http://.../api/android/MoneyBalance
```

Input:

```json
-
```

Output:

```json
{
    "result": true,
    "MoneyBalance": 0
}
```

# Questionnaires History

URL:

```c#
http://.../api/android/Responses
```

Input:

```json
-
```

Output:

```json
{
    "responses": [
        {
            "QuestionnaireName": "new",
            "QuestionnairePrice": 1400,
            "DateFinished": "2018-12-18T00:42:28.003"
        },
        {
            "QuestionnaireName": "پرسشنامه جدید",
            "QuestionnairePrice": 90000,
            "DateFinished": "2018-12-18T00:42:28.003"
        }
    ]
}
```

# Notifications

URL:

```c#
http://.../api/android/Notifications
```

Input:

```json
-
```

Output:

```json
{
    "Notifications": [
        {
            "DateCreated": "2018-12-18T00:38:52.027",
            "Title": "پرداخت",
            "Text": "مبلغ 541400 تومان به حساب بانکی شما پرداخت گردید.",
            "Type": 1
        },
        {
            "DateCreated": "2018-12-18T00:42:28.037",
            "Title": "پرداخت",
            "Text": "مبلغ 541400 تومان به حساب بانکی شما پرداخت گردید.",
            "Type": 1
        },
        {
            "DateCreated": "2018-12-18T00:43:03.053",
            "Title": "پرداخت",
            "Text": "مبلغ 360000 تومان به حساب بانکی شما پرداخت گردید.",
            "Type": 1
        },
        ...
        ]
 }
```

