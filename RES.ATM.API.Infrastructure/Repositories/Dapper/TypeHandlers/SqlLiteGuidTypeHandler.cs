using Dapper;
using System;
using System.Data;

namespace RES.ATM.API.Infrastructure.Repositories.Dapper.TypeHandlers
{
    public class SqlLiteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value) => Guid.Parse(((string)value).ToUpper());

        public override void SetValue(IDbDataParameter parameter, Guid value) => parameter.Value = value;      
    }
}
