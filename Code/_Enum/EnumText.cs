public enum ELanguageType
{
    None = 0,

    Eng = 1,        // 영어
    Kor = 2,        // 한국어
    Jpa = 3,        // 일본어
    Chi_Hant = 4,   // 중국어 - 번체
    Chi_Hans = 5,   // 중국어 - 간체

    Max,
}

public enum ETextType
{
    None = 0,

    Game = 1,
    Shop = 2,
    Car = 3,
    Paint = 4,
    Parts = 5,

    Max,
}

public enum EGameText
{
    None = 0,

    Error = 1, // 에러가 발생했습니다. (code : %s + )

    Max,
}

public enum EShopText
{
    None = 0,

    Shop_0 = 1, // 구매에 성공했습니다.
    Shop_1 = 2, // 이미 보유하고 있는 아이템입니다.
    Shop_2 = 3, // 보유한 코인이 부족합니다.
    Shop_3 = 4, // {0} 코인으로 구매하시겠습니까?

    Max,
}

public enum ECarText
{
    None = 0,

    Car_1 = 1001, // CAR1
    Car_2 = 1002, // CAR2
    Car_3 = 1003, // CAR3
    Car_4 = 1004, // CAR4

    Max,
}

public enum EPaintText
{
    None = 0,

    Paint_1 = 1001, // 오렌지
    Paint_2 = 1002, // 파랑
    Paint_3 = 1003, // 아이보리
    Paint_4 = 1004, // 빨강

    Max,
}

public enum EPartsText
{
    None = 0,

    Parts_1 = 1001, // 흔한 타이어
    Parts_2 = 1002, // 허름한 엔진
    Parts_3 = 1003, // 오래된 센서
    Parts_4 = 1004, // 조잡한 바디킷

    Max,
}