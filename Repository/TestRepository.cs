using IRepository;
using Model.Models;
using Repository.sugar;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    public class TestRepository :BaseRepository<Notes>, ITestRepository
    {
        //private DbContext context;
        //private SqlSugarClient db;
        //private SimpleClient<Notes> entityDB;

        //internal SqlSugarClient Db
        //{
        //    get { return db; }
        //    private set { db = value; }
        //}
        //public DbContext Context
        //{
        //    get { return context; }
        //    set { context = value; }
        //}
        //public TestRepository()
        //{
        //    DbContext.Init(BaseDBConfig.ConnectionString);
        //    context = DbContext.GetDbContext();
        //    db = context.Db;
        //    entityDB = context.GetEntityDB<Notes>(db);
        //}



        //public int Add(Notes model)
        //{
        //    var i = db.Insertable(model).ExecuteReturnIdentity();
        //    return i.ObjToInt();
        //}

        //public bool Delete(Notes model)
        //{
        //    var i = db.Deleteable(model).ExecuteCommand();
        //    return i > 0;
        //}

        //public List<Notes> Query(Expression<Func<Notes, bool>> whereExpression)
        //{
        //    return entityDB.GetList(whereExpression);
        //}

    

        //public bool Update(Notes model)
        //{
        //    //这种方式会以主键为条件
        //    var i = db.Updateable(model).ExecuteCommand();
        //    return i > 0;
        //}

        public int Sum(int i, int j)
        {
            return i + j;
        }

    }
}
