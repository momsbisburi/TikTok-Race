using System;
using System.Collections;

namespace TikTok_Race
{
	// Token: 0x02000002 RID: 2
	public class myReverserClass : IComparer
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(y, x);
		}
	}
}
