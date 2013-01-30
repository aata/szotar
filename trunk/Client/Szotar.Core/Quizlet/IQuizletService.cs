using System.ComponentModel;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Szotar.Quizlet {
    public interface IQuizletService {
        Task<Credentials> Authenticate(string code, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null);
        void Authenticate(Credentials credentials);
        Credentials? Credentials { get; }
        Uri GetLoginPageUri(out string randomState);

        Task<SetModel> FetchSetInfo(long setID, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null);
        Task<List<SetModel>> FetchSetInfo(IEnumerable<long> setIDs, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null);

        Task<List<SetModel>> SearchSets(string query, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null);

        Task<UserModel> FetchUserInfo(string userName, CancellationToken token, IProgress<ProgressChangedEventArgs> progress = null);
        Task<List<SetModel>> FetchUserSets(string userName, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null);

	    Task UpdateSet(SetModel set, CancellationToken cancel, IProgress<ProgressChangedEventArgs> progress = null);
    }
}