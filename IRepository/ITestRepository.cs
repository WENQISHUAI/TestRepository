using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace IRepository
{
    public interface ITestRepository:IBaseRepository<Notes>
    {

        int Sum(int i, int j);


        //int Add(Notes model);
        //bool Delete(Notes model);
        //bool Update(Notes model);
        //List<Notes> Query(Expression<Func<Notes, bool>> whereExpression);
    }
}
