using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace QuestionnaireProject.Areas.Questioner.Persistence
{
    public class MediaRepository:IMediaRepository
    {
        private readonly QuestionnaireEntities _context;
        public MediaRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public Media Create(string questionId, string url)
        {
            var newMedia = new Media()
            {
                Question_Id = questionId,
                Url = url
            };           
            return _context.Medias.Add(newMedia);
        }

        public Media Edit(string questionId, string url)
        {
            var media = GetByQuestionId(questionId);
            if (media!=null)
            {
                media.Url = url;
            }
            else
            {
                Create(questionId, url);
            }
            return media;
        }

        public Media GetByQuestionId(string questionId)
        {
            var mediaId = _context.Medias.Where(m => m.Question_Id == questionId).Select(m => m.Id).FirstOrDefault();
            var media = GetById(mediaId);
            return media;
        }

        public Media GetById(int mediaId)
        {
            var media = _context.Medias.Find(mediaId);
            return media;
        }

        public IEnumerable<Media> GetAllMedias(string questionnaireId)
        {
            var medias = _context.Medias.Where(m => m.Question.Questionnaire_Id == questionnaireId);
            return medias;
        }

        public void Delete(string questionId)
        {
            var media = GetByQuestionId(questionId);
            if (media!=null)
            {
                _context.Medias.Remove(media);
            }
        }

        public string CreatePictureLink(HttpPostedFileBase file, HttpServerUtilityBase server, string prefix)
        {
            var fileName = prefix + Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
            string path = Path.Combine(server.MapPath("~/Uploads/QuestionnairePhotos"), fileName);
            file.SaveAs(path);
            return fileName;           
        }
    }
}