using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SandBeige.MediaBox.Library.Expressions {

	/// <summary>
	/// パラメータ上書きクラス
	/// </summary>
	public class ParameterVisitor : ExpressionVisitor {
		/// <summary>
		/// パラメータ保持
		/// </summary>
		private readonly IDictionary<(Type, string), ParameterExpression> _parameters;

		/// <summary>
		/// パラメータ
		/// </summary>
		public ICollection<ParameterExpression> Parameters {
			get {
				return this._parameters.Values;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="parameters">上書きするパラメータ</param>
		public ParameterVisitor(IEnumerable<ParameterExpression> parameters) {
			this._parameters = parameters.ToDictionary(p => (p.Type, p.Name));
		}

		/// <summary>
		/// パラメータ選択
		/// </summary>
		/// <remarks>
		/// 対象のパラメータと同一型、同一名のパラメータを保持していれば上書きする。
		/// </remarks>
		/// <param name="node">対象パラメータ</param>
		/// <returns>上書きするパラメータ</returns>
		protected override Expression VisitParameter(ParameterExpression node) {
			var key = (node.Type, node.Name);
			return this._parameters.ContainsKey(key)
				? this._parameters[key]
				: node;
		}
	}
}
