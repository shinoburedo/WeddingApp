using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MauloaDemo.Utilities {

    public class SqlParamSet {

        private HashSet<SqlParameter> mList = new HashSet<SqlParameter>();

        public SqlParameter[] ToArray() {
            return mList.ToArray();
        }

        public void Clear() {
            mList.Clear();
        }

        public SqlParameter Get(string paramName) {
            return mList.FirstOrDefault(p => p.ParameterName == paramName);
        }

        public SqlParameter AddVarChar(
                    string paramName,
                    int size,
                    string value,
                    bool nullable) {

            if (!nullable && value == null) {
                value = string.Empty;
            }
            return AddVarChar(paramName, size, value, ParameterDirection.Input, nullable);
        }

        public SqlParameter AddVarChar(
                    string paramName,
                    int size,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input,
                    bool nullable = true) {

            var prm = new SqlParameter(paramName, SqlDbType.VarChar);
            prm.Size = size;
            prm.Value = nullable ? TypeHelper.EmptyToDBNull(value) : value;
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddVarCharMax(
                    string paramName,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.VarChar, -1);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddChar(
                    string paramName,
                    int size,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.Char);
            prm.Size = size;
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddNVarChar(
                    string paramName,
                    int size,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.NVarChar);
            prm.Size = size;
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddNVarCharMax(
                    string paramName,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.NVarChar, -1);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddNChar(
                    string paramName,
                    int size,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.NChar);
            prm.Size = size;
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddText(
                    string paramName,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.Text);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddNText(
                    string paramName,
                    string value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.NText);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddInt(
                    string paramName,
                    int? value,
                    ParameterDirection direction = ParameterDirection.Input) {


            var prm = new SqlParameter(paramName, SqlDbType.Int);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddInt(
                    string paramName,
                    int value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddInt(paramName, (int?)value, direction);
        }

        public SqlParameter AddSmallInt(
                    string paramName,
                    short? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.SmallInt);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddSmallInt(
                    string paramName,
                    short value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddSmallInt(paramName, (short?)value, direction);
        }

        public SqlParameter AddBigInt(
                    string paramName,
                    Int64? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.BigInt);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddBigInt(
                    string paramName,
                    Int64 value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddBigInt(paramName, (Int64?)value, direction);
        }

        public SqlParameter AddTinyInt(
                    string paramName,
                    byte? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.TinyInt);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddTinyInt(
                    string paramName,
                    byte value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddTinyInt(paramName, (byte?)value, direction);
        }

        public SqlParameter AddBit(
                    string paramName,
                    bool? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.Bit);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddBit(
                    string paramName,
                    bool value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddBit(paramName, (bool?)value, direction);
        }

        public SqlParameter AddDecimal(
                    string paramName,
                    decimal? value,
                    byte precision = 18,
                    byte scale = 2,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.Decimal);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Precision = precision;
            prm.Scale = scale;
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddDecimal(
                    string paramName,
                    decimal value,
                    byte precision = 18,
                    byte scale = 2,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddDecimal(paramName, (decimal?)value, precision, scale, direction);
        }

        public SqlParameter AddFloat(
                    string paramName,
                    float? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.Float);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddFloat(
                    string paramName,
                    float value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddFloat(paramName, (float?)value, direction);
        }

        public SqlParameter AddMoney(
                    string paramName,
                    decimal? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.Money);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddMoney(
                    string paramName,
                    decimal value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddMoney(paramName, (decimal?)value, direction);
        }

        public SqlParameter AddDateTime(
                    string paramName,
                    DateTime? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.DateTime);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddDateTime(
                    string paramName,
                    DateTime value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddDateTime(paramName, (DateTime?)value, direction);
        }

        public SqlParameter AddSmallDateTime(
                    string paramName,
                    DateTime? value,
                    ParameterDirection direction = ParameterDirection.Input) {

            var prm = new SqlParameter(paramName, SqlDbType.SmallDateTime);
            prm.Value = TypeHelper.EmptyToDBNull(value);
            prm.Direction = direction;
            if (mList != null) mList.Add(prm);
            return prm;
        }

        public SqlParameter AddSmallDateTime(
                    string paramName,
                    DateTime value,
                    ParameterDirection direction = ParameterDirection.Input) {

            return AddSmallDateTime(paramName, (DateTime?)value, direction);
        }


    }
}
