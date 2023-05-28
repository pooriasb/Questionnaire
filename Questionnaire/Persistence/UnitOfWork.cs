using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;

namespace QuestionnaireProject.Persistence
{
    public class UnitOfWork<C>:IUnitOfWork<C> where C: QuestionnaireEntities, new()
    {
        private readonly GenericRepository<C,QuestionnaireEntities> genericRepository;

        public UnitOfWork()
        {
            //genericRepository = new GenericRepository<C,QuestionnaireEntities>();
        }
        public void Complete()
        {
            throw new NotImplementedException();
        }
    }

    //todo : https://softwareengineering.stackexchange.com/questions/365928/generic-repository-pattern-ef-and-unit-of-work
}