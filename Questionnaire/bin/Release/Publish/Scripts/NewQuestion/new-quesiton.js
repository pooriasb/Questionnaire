var answerListGlobal;
console.log('hi');
$('textarea').on('input',
    function () {
        var textArea = $(this);
        $('#answers').remove();
        $('#maxLength').remove();
        var answersList = (textArea.val()).split('|', 6);
        answerListGlobal = answersList;
        if (!answersList.includes('') && answersList.length >= 2) {
            $('<ol id="answers"></ol>').insertAfter(textArea);
            answersList.forEach(function (element) {

                $("#answers").append('<li class="text-green"><b>' + element + '</b></li>');
            });
        } else {
            $(
                '<div class="text-red" id="answers">پاسخها نمیتواند حاوی گزینه خالی باشد. در این صورت سوال شما به صورت تشریحی در نظر گرفته میشود</div>')
                .insertAfter(textArea);

        }

        if (answersList.length >= 6) {
            $('<div class="text-red" id="maxLength">حداکثر شش گزینه را میتوانید وارد نمایید</div>')
                .insertAfter(textArea);
        }

    });

$('textarea').on('focusout', function () {
    var textArea = $(this);
    $('select[id="answersSelect"]').empty();
    if (answerListGlobal.length >= 2) {
        console.log(answerListGlobal);
        answerListGlobal.forEach(function (element, index) {
            $('select[id="answersSelect"]').prop("disabled", false);
            $('select[id="answersSelect"]').append('<option class="answerElement" value="' +
                index +
                '"><b> گزینه' +
                (index + 1) +
                '</b></li>');
        });
    } else {
        $('select[id="answersSelect"]').prop("disabled", true);
    }

});

