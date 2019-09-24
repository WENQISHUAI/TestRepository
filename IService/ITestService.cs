using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface ITestService : IBaseService<Notes>
    {
        int Sum(int i, int j);

        Task<int> Add(Notes model);
        Task<bool> Delete(Notes model);
        Task<bool> Update(Notes model);
        Task<List<Notes>> Query(Expression<Func<Notes, bool>> whereExpression);
    }
}
