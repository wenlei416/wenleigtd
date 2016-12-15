using System.Collections.Generic;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class ContextServices : IContextServices
    {
        private readonly IContextRepository _contextRepository;

        public ContextServices(IContextRepository contextRepository)
        {
            _contextRepository = contextRepository;
        }

        public Context GetContextById(int id)
        {
            return _contextRepository.GetContextById(id);
        }

        public void CreateContext(Context context)
        {
            _contextRepository.Create(context);
        }

        public IEnumerable<Context> GetAllContexts()
        {
            return _contextRepository.GetAll();
        }

        public void UpdateContext(Context context)
        {
            _contextRepository.Update(context);
        }

        public void DeleteContext(Context context)
        {
            _contextRepository.Delete(context);

        }

        public int? IsExistByName(string contextName)
        {
            var con = _contextRepository.Get(c => c.ContextName == contextName);
            return con == null ? (int?)null : _contextRepository.Get(c => c.ContextName == contextName).ContextId;
        }
    }
}