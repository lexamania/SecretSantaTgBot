namespace SecretSantaTgBot.Localization.Dictionaries;

public class MessagesDictionaryUA : IMessagesDictionary
{
    public string CommandError { get; } = "❌ Не роспізнана команда! ❌";
    public string CommandBotMenu { get; } = "☃️ Меню Бота ☃️\n";
    public string CommandHelp { get; } = "сторінка з усіма командами";
    public string CommandStop { get; } = "перейти до головного меню";

    public string CommandUpdateRoom { get; } = "змінити опис кімнати";
    public string CommandLeaveRoom { get; } = "зупинити участь і покинути кімнату";
    public string CommandShowRoomInfo { get; } = "показати інформацію по кімнаті";
    public string CommandShowTarget { get; } = "показати мою ціль на Таємного Санту";
    public string CommandStartWishes { get; } = "розпочати додавати бажання";
    public string CommandStopWishes { get; } = "зупинити додавати бажання";
    public string CommandClearWishes { get; } = "очистити список своїх бажань";
    public string CommandShowMe { get; } = "показати інформацію про мене";
    public string CommandBack { get; } = "повернутися до списку кімнат";

    public string CommandCreateRoom { get; } = "створити нову кімнату";
    public string CommandSelectRoom { get; } = "перейти до кімнати";
    public string CommandDeleteRoom { get; } = "видалити кімнату";
    public string CommandShowRooms { get; } = "показати список моїх кімнат";
    
    public string CommandStartSanta { get; } = "Розпочинаємо крутити барабан на Санту🎰!";
    public string CommandNotifyEveryone { get; } = "оповістити кожного в кімнаті";

    public string ZeroRooms { get; } = "В тебе немає кімнат!";
    public string RoomCreationEnterTitle { get; } = "Введіть назву кімнати:";
    public string RoomCreationEnterDescription { get; } = "Введіть опис івенту:";
    public string ChooseRoom { get; } = "Виберіть кімнату:";
    public string EnterRealName { get; } = "Введіть ваше ім'я:";
    public string RoomDoesntExist { get; } = "❌ Кімнати з таким кодом не знайдено!";
    public string RoomCreated { get; } = "Кімната створена!\n\nКод кімнати:";
    public string RoomDeleted { get; } = "- кімнату видалено! 🥳";
    public string RoomsList { get; } = "Список твоїх кімнат:";
    public string RoomDescriptionUpdated { get; } = "Опис кімнати оновлено! 🥳";

    public string NeedAdminRights { get; } = "❌ Ви не адміністратор кімнати!";
    public string SecretSantaWasPlayed { get; } = "❌ Таємний Санта вже був розіграний!";
    public string UserLeavedRoom { get; } = "🗿 Вас викреслено з участі! 🗿";
    public string UserLeavedRoomForAll { get; } = "покинув кімнату! 🗿";
    public string NotEnoughParticipants { get; } = "Недостатньо участників для розподілення!";
    public string UserNewParticipation { get; } = "🥳 Розпочинаємо реєстрацію, введіть ваше ім'я:";
    public string UserParticipationEnd { get; } = "🥳 Вас зареєстровано! 🥳";
    public string ParticipantsList { get; } = "🎅 Список учасників ";
    public string AdminCantLeaveRoom { get; } = "Ви як адміністратор не можете покинути цю кімнату!\nЯкщо треба ви можете видалити її з основного меню.";

    public string UserTarget { get; } = "🎅 Ваша ціль:";
    public string TargetHaveZeroWishes { get; } = "🔕 Твоя ціль не вказала бажання 🔕";
    public string UserHaveZeroWishes { get; } = "🔕 Ти ще не заповнив список бажань! 🔕";
    public string UserWishesCleared { get; } = "Ваші побажання очищено❗️";
    public string UserWishesList { get; } = "🎁 Список твоїх бажань 🎁";
    public string TargetWishesList { get; } = "🎁 Список бажань твоєї цілі 🎁";
    public string UserStartWishes { get; } = """
        🎁 <b>Почніть додавати ваші побажання.</b> 🎁

        <b><u>Правила</u></b>:
        1. Можете всі побажання відправити текстом за 1 повідомлення
        2. Можете відправити текст з прикріпленними картинками
        3. Чи просто самі картинки
        4. Для завершення виберіть команду /stop чи будь-яку іншу
        """;
    public string UserStopWishes { get; } = "🎁 Ваші побажання збережено! 🥳";
    public string UserWishAdded { get; } = "🎁 Побажання додано! 🥳 \n\n Продовжіть додавати побажання чи завершіть зараз /stop";

    public string EnterInRoomMessage { get; } = "Введіть повідомлення:";
    public string InRoomMessageSend { get; } = "Повідомлення відправлено!";
    public string SecretSantaStillOffline { get; } = "🛎 Розподілення Санти ще не розпочалося! 🛎";
    public string SantaFinished { get; } = "Вибір Санти завершився. Перейдіть в кімнату аби подивитися хто став вашою ціллю 🥳";
    public string RoomNumber { get; } = "Кімната -";
    
    public string AskConfirmation { get; } = "Ви впевнені? (Так/Ні)";
    public string ButtonYes { get; } = "Так";
    public string ButtonNo { get; } = "Ні";
}
