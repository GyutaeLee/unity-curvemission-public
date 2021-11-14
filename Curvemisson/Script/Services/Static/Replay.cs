using System.IO;

namespace Services.Static
{
    public static class Replay
    {
        public static bool IsReplayMode { get; private set; }
        public static bool IsUserReplay { get; private set; }

        public static void ActiveUserReplayMode()
        {
            IsReplayMode = true;
            IsUserReplay = true;
        }

        public static void ActiveOtherUserReplayMode()
        {
            IsReplayMode = true;
            IsUserReplay = false;
        }

        public static void InActiveReplayMode()
        {
            IsReplayMode = false;
        }

        public static bool IsUserReplayFileExist(int stageID)
        {
            string replayFilePath = GetUserSingleRacingReplayFilePath(stageID);
            FileInfo replayFileInfo = new FileInfo(replayFilePath);

            return replayFileInfo.Exists;
        }

        public static string GetUserSingleRacingReplayFilePath(int stageID)
        {
            return Constants.Replay.ReplayDirectoryPath + "/" + GetUserSingleRacingReplayFileName(stageID);
        }

        public static string GetUserSingleRacingReplayFileName(int stageID)
        {
            const string userSingleRacingReplayFileName = "security-related";
            return userSingleRacingReplayFileName + "_" + stageID;
        }

        public static string GetOtherUserSingleRacingReplayFilePath()
        {
            const string otherUserSingleRacingReplayFileName = "security-related";
            return Constants.Replay.ReplayDirectoryPath + "/" + otherUserSingleRacingReplayFileName;
        }
    }
}
