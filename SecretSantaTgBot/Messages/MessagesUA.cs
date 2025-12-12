namespace SecretSantaTgBot.Messages;

public class MessagesUA : MessagesBase
{
    public override string CommandError { get; } = "❌ Не роспізнана команда! ❌";
    public override string CommandBotMenu { get; } = "☃️ Меню Бота ☃️\n";
    public override string CommandHelp { get; } = "сторінка з усіма командами";
    public override string CommandStop { get; } = "перейти до головного меню";

    public override string CommandUpdateRoom { get; } = "змінити опис кімнати";
    public override string CommandLeaveRoom { get; } = "зупинити участь і покинути кімнату";
    public override string CommandShowRoomInfo { get; } = "показати інформацію по кімнаті";
    public override string CommandShowTarget { get; } = "показати мою ціль на Таємного Санту";
    public override string CommandStartWishes { get; } = "розпочати додавати бажання";
    public override string CommandStopWishes { get; } = "зупинити додавати бажання";
    public override string CommandClearWishes { get; } = "очистити список своїх бажань";
    public override string CommandShowMe { get; } = "показати інформацію про мене";
    public override string CommandBack { get; } = "повернутися до списку кімнат";

    public override string CommandCreateRoom { get; } = "створити нову кімнату";
    public override string CommandSelectRoom { get; } = "перейти до кімнати";
    public override string CommandDeleteRoom { get; } = "видалити кімнату";
    public override string CommandShowRooms { get; } = "показати список моїх кімнат";

    public override string ZeroRooms { get; } = "В тебе немає кімнат!";
    public override string RoomCreationEnterTitle { get; } = "Введіть назву кімнати:";
    public override string RoomCreationEnterDescription { get; } = "Введіть опис івенту:";
    public override string ChooseRoom { get; } = "Виберіть кімнату:";
    public override string EnterRealName { get; } = "Введіть ваше ім'я:";
    public override string RoomDoesntExist { get; } = "❌ Кімнати з таким кодом не знайдено!";
    public override string RoomCreated { get; } = "Кімната створена!\n\nКод кімнати:";
    public override string RoomDeleted { get; } = "- кімнату видалено! 🥳";
    public override string RoomsList { get; } = "Список твоїх кімнат:";
    public override string RoomDescriptionUpdated { get; } = "Опис кімнати оновлено! 🥳";

    public override string UserLeavedRoom { get; } = "🗿 Вас викреслено з участі! 🗿";
    public override string UserLeavedRoomForAll { get; } = "покинув кімнату! 🗿";
    public override string NeedAdminRights { get; } = "❌ Ви не адміністратор кімнати!";
    public override string NotEnoughParticipants { get; } = "Недостатньо участників для розподілення!";
    public override string UserNewParticipation { get; } = "🥳 Розпочинаємо реєстрацію, введіть ваше ім'я:";
    public override string UserParticipationEnd { get; } = "🥳 Вас зареєстровано! 🥳";
    public override string ParticipantsList { get; } = "🎅 Список учасників ";
    public override string AdminCantLeaveRoom { get; } = "Ви як адміністратор не можете покинути цю кімнату!\nЯкщо треба ви можете видалити її з основного меню.";

    public override string UserTarget { get; } = "🎅 Ваша ціль:";
    public override string TargetHaveZeroWishes { get; } = "🔕 Твоя ціль не вказала бажання 🔕";
    public override string UserHaveZeroWishes { get; } = "🔕 Ти ще не заповнив список бажань! 🔕";
    public override string UserWishesCleared { get; } = "Ваші побажання очищено❗️";
    public override string UserWishesList { get; } = "🎁 Список твоїх бажань 🎁";
    public override string TargetWishesList { get; } = "🎁 Список бажань твоєї цілі 🎁";
    public override string UserStartWishes { get; } = """
        🎁 <b>Почніть додавати ваші побажання.</b> 🎁

        <b><u>Правила</u></b>:
        1. Можете всі побажання відправити текстом за 1 повідомлення
        2. Можете відправити текст з прикріпленними картинками
        3. Чи просто самі картинки
        4. Для завершення виберіть команду /stop чи будь-яку іншу
        """;
    public override string UserStopWishes { get; } = "🎁 Ваші побажання збережено! 🥳";
    public override string UserWishAdded { get; } = "🎁 Побажання додано! 🥳 \n\n Продовжіть додавати побажання чи завершіть зараз /stop";

    public override string SecretSantaStillOffline { get; } = "🛎 Розподілення Санти ще не розпочалося! 🛎";
    public override string StartSanta { get; } = "Розпочинаємо крутити барабан на Санту🎰!";
    public override string SantaFinished { get; } = "Вибір Санти завершився. Перейдіть в кімнату аби подивитися хто став вашою ціллю 🥳";
    public override string RoomNumber { get; } = "Кімната -";
}
