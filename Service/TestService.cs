using IRepository;
using IService;
using Model.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TestService :BaseService<Notes>, ITestService
    {



        //public ITestRepository dal = new TestRepository();

        ITestRepository dal;

        public TestService(ITestRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }

        public Task<int> Add(Notes model)
        {
         return   dal.Add(model);
        }

        public Task<bool> Delete(Notes model)
        {
            return dal.Delete(model);
        }

        public Task< List<Notes>> Query(Expression<Func<Notes, bool>> whereExpression)
        {
            return dal.Query(whereExpression);
        }

        public int Sum(int i, int j)
        {
            return dal.Sum(i, j);
        }

        public Task<bool> Update(Notes model)
        {
            return dal.Update(model);
        }
    }
}
