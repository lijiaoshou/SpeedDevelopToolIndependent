using System;

namespace CVST.AppFramework.AssemblyLoader
{
	/// <summary>
	/// LoadException ��ժҪ˵����
	/// </summary>
	public class AssemblyLoadFailureException:Exception
	{
		public AssemblyLoadFailureException()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		public override string Message
		{
			get
			{
				return "Assembly Load Failure";
			}
		}

	}
}
