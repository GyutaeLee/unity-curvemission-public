namespace Services.Enum.GameText
{
    public enum LanguageType
    {
        Eng = 0,         // 영어
        Kor = 1,         // 한국어
        Jpa = 2,         // 일본어
        Chi_Hant = 3,    // 중국어 - 번체
        Chi_Hans = 4,    // 중국어 - 간체

        Max,
    }

    public enum TextType
    {
        None = 0,

        Game = 1,
        Shop = 2,
        CarItemCar = 3,
        CarItemPaint = 4,
        CarItemParts = 5,
        Stage = 6,
        AvatarItemHead = 7,
        AvatarItemTop = 8,
        AvatarItemBottom = 9,

        Max,
    }

    public enum Game
    {
        None = 0,

        Error = 1, // 에러가 발생했습니다. (code : {0})

        Max,
    }

    public enum Shop
    {
        None = 0,

        Shop_0 = 1, // 구매에 성공했습니다.
        Shop_1 = 2, // 이미 보유하고 있는 아이템입니다.
        Shop_2 = 3, // 보유한 코인이 부족합니다.
        Shop_3 = 4, // {0} 코인으로 구매하시겠습니까?

        Max,
    }

    public enum CarItemCar
    {
        None = 0,

        Car_1 = 1001, // CAR1
        Car_2 = 1002, // CAR2
        Car_3 = 1003, // CAR3
        Car_4 = 1004, // CAR4

        Max,
    }

    public enum CarItemPaint
    {
        None = 0,

        Paint_1 = 1001, // 오렌지
        Paint_2 = 1002, // 파랑
        Paint_3 = 1003, // 아이보리
        Paint_4 = 1004, // 빨강

        Max,
    }

    public enum CarItemParts
    {
        None = 0,

        Parts_1 = 1001, // 흔한 타이어
        Parts_2 = 1002, // 허름한 엔진
        Parts_3 = 1003, // 오래된 센서
        Parts_4 = 1004, // 조잡한 바디킷

        Max,
    }

    public enum Stage
    {
        None = 0,

        Stage01 = 1001, // 발산마을
        Stage02 = 1002, // 맵02
        Stage03 = 1003, // 맵03
        STage04 = 1004, // 맵04

        Max,
    }

    public enum AvatarItemHead
    {
        None = 0,

        Head_1 = 1001,
        Head_2 = 1002,
        Head_3 = 1003,
        Head_4 = 1004,
        Head_5 = 1005,
        Head_6 = 1006,
        Head_7 = 1007,
        Head_8 = 1008,

        Max,
    }
    public enum AvatarItemTop
    {
        None = 0,

        Top_1 = 1001,
        Top_2 = 1002,
        Top_3 = 1003,
        Top_4 = 1004,
        Top_5 = 1005,
        Top_6 = 1006,
        Top_7 = 1007,
        Top_8 = 1008,

        Max,
    }

    public enum AvatarItemBottom
    {
        None = 0,

        Bottom_1 = 1001,
        Bottom_2 = 1002,
        Bottom_3 = 1003,
        Bottom_4 = 1004,
        Bottom_5 = 1005,
        Bottom_6 = 1006,
        Bottom_7 = 1007,
        Bottom_8 = 1008,

        Max,
    }
}