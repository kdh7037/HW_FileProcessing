#include <stdio.h>

typedef struct Record
{ //구조체 선언
    int account;
    char name[16];
    char address[41];
    char car[11];
    char work[11];
    char phone[9];
    int code;
}
Record;

Record master;
Record transaction;

FILE *masterfile;
FILE *transactionfile;
FILE *newmaster;

void create_master()
{ //마스터파일생성
    int account;
    char name[16];
    char address[41];
    char car[11];
    char work[11];
    char phone[9];

    FILE *cfPtr;

    if ((cfPtr = fopen("master.dat", "w")) == NULL)
    {
        printf("File could not be opened\n");
    }
    else
    {
        printf("Make Master File\n");
        printf("Enter the account, name, address, car, work, and phone.\n");
        printf("Enter EOF to end input.\n");
        printf("? ");
        scanf("%d%s%s%s%s%s", &account, name, address, car, work, phone);

        while (!feof(stdin))
        {
            if (ftell(cfPtr) != 0)
            {
                fprintf(cfPtr, "\n");
            }
            fprintf(cfPtr, "%d %s %s %s %s %s", account, name, address, car, work, phone);
            printf("? ");
            scanf("%d%s%s%s%s%s", &account, name, address, car, work, phone);
        }
        fclose(cfPtr);
    }
}

void create_transaction()
{ //트랜잭션파일 생성
    int account;
    char name[16];
    char address[41];
    char car[11];
    char work[11];
    char phone[9];
    int code;

    FILE *cfPtr;

    if ((cfPtr = fopen("transaction.dat", "w")) == NULL)
    {
        printf("File could not be opened\n");
    }
    else
    {
        printf("Make Transaction File\n");
        printf("Enter the account, name, address, car, work, phone, and code.\n");
        printf("Enter EOF to end input.\n");
        printf("? ");
        scanf("%d%s%s%s%s%s%d", &account, name, address, car, work, phone, &code);

        while (!feof(stdin))
        {
            if (ftell(cfPtr) != 0)
            {
                fprintf(cfPtr, "\n");
            }
            fprintf(cfPtr, "%d %s %s %s %s %s %d", account, name, address, car, work, phone, code);
            printf("? ");
            scanf("%d%s%s%s%s%s%d", &account, name, address, car, work, phone, &code);
        }
        fclose(cfPtr);
    }
}

void nexttrans()
{ //다음 트랜잭션레코드로 이동
    if (feof(transactionfile))
        transaction.account = EOF;
    else
        fscanf(transactionfile, "%d %s %s %s %s %s %d", &transaction.account, transaction.name, transaction.address, transaction.car, transaction.work, transaction.phone, &transaction.code);
}

void nextmaster()
{ //다음 마스터레코드로 이동
    if (feof(masterfile))
        master.account = EOF;
    else
        fscanf(masterfile, "%d %s %s %s %s %s", &master.account, master.name, master.address, master.car, master.work, master.phone);
}

void sequentialUpdate()
{
    masterfile = fopen("master.dat", "r");
    transactionfile = fopen("transaction.dat", "r");
    newmaster = fopen("newmaster.dat", "w");

    nexttrans();
    nextmaster();

    while (master.account != EOF && transaction.account != EOF)
    {
        if (master.account < transaction.account)
        {
            fprintf(newmaster, "%d %s %s %s %s %s\n", master.account, master.name, master.address, master.car, master.work, master.phone);
            nextmaster();
        }
        else if (master.account == transaction.account)
        {
            switch (transaction.code)
            {
            case 0:
                nexttrans();
                break;
            case 1:
                fprintf(newmaster, "%d %s %s %s %s %s\n", master.account, master.name, master.address, transaction.car, master.work, master.phone);
                nextmaster();
                nexttrans();
                break;
            case 2:
                nextmaster();
                nexttrans();
                break;
            case 3:
                fprintf(newmaster, "%d %s %s %s %s %s\n", master.account, master.name, transaction.address, master.car, master.work, master.phone);
                nextmaster();
                nexttrans();
                break;
            case 4:
                fprintf(newmaster, "%d %s %s %s %s %s\n", master.account, master.name, master.address, master.car, master.work, transaction.phone);
                nextmaster();
                nexttrans();
                break;
            case 5:
                fprintf(newmaster, "%d %s %s %s %s %s\n", master.account, master.name, master.address, master.car, transaction.work, master.phone);
                nextmaster();
                nexttrans();
                break;
            default:
                nexttrans();
                break;
            }
        }
        else
        {
            switch (transaction.code)
            {
            case 0:
                fprintf(newmaster, "%d %s %s %s %s %s\n", transaction.account, transaction.name, transaction.address, transaction.car, transaction.work, transaction.phone);
                nexttrans();
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            default:
                nexttrans();
                break;
            }
        }
    }

    if (master.account == EOF)
    {
        while (transaction.account != EOF)
        {
            switch (transaction.code)
            {
            case 0:
                fprintf(newmaster, "%d %s %s %s %s %s\n", transaction.account, transaction.name, transaction.address, transaction.car, transaction.work, transaction.phone);
                nexttrans();
                break;
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            default:
                nexttrans();
                break;
            }
        }
    }
    if (transaction.account == EOF)
    {
        while (master.account != EOF)
        {
            fprintf(newmaster, "%d %s %s %s %s %s\n", master.account, master.name, master.address, master.car, master.work, master.phone);
            nextmaster();
            if (master.account == EOF)
                fputc('\n', newmaster);
        }
    }

    fclose(masterfile);
    fclose(transactionfile);
    fclose(newmaster);
}

int main()
{
    create_master();
    create_transaction();
    sequentialUpdate();
    getchar(); //함수 강제종료 방지
    return 0;
}