using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashing
{
    class Hashing
    {
        public class Directory  //디렉토리 클래스
        {
            public Bucket[] directory = new Bucket[2];  //버킷 사이즈의 2칸짜리 초기 디렉토리를 생성
            private Record[] records = new Record[3];   //레코드의 이동을 위해 사용할 배열
            public int length;                          //레코드의 크기 저장

            public Directory()  //디렉토리에 버킷을 넣는다.
            {
                directory[0] = new Bucket();
                directory[1] = new Bucket();
                length = directory.Length;
            }

            public void NewRecord(Record A, int step, uint original)
            //버킷이 가득 찬 상태에서 새로운 레코드가 들어올 때 실시 될 메소드
            {
                A.all_bit_combination(step);    //A레코드의 디렉토리에 들어가기 전 비트 계산


                records[0] = directory[original].record1;  //가득 찬 버킷의 레코드들을 옮겨넣습니다.
                records[1] = directory[original].record2;
                records[2] = directory[original].record3;


                directory[original].record1 = directory[original].record2 = directory[original].record3 = null;
                //그 후 버킷 null로 초기화

                records[0].bit_combination();   //각각의 레코드 비트 한단계 증가
                records[1].bit_combination();
                records[2].bit_combination();
                A.bit_combination();


                if (records[0].realbit >= directory.Length || records[1].realbit >= directory.Length || records[2].realbit >= directory.Length || A.realbit >= directory.Length)
                //디렉토리가 늘어야 할지 안늘어야 할지 확인 
                {
                    for (; records[0].realbit >= directory.Length || records[1].realbit >= directory.Length || records[2].realbit >= directory.Length || A.realbit >= directory.Length;)
                    //디렉토리가 수용할 수 있을 때까지 반복 증가
                    {
                        Console.WriteLine("Directory가 2배가 되었습니다.");
                        Array.Resize(ref directory, directory.Length * 2);  //디렉토리 크기 2배로 증가
                        for (int i = length; i < directory.Length; i++)
                            //그 후, 새로생긴 디렉토리배열 전부 Bucket()으로 만든다.
                            directory[i] = new Bucket();
                        length = directory.Length;                          //새 길이 저장
                    }
                }
                else
                    Console.WriteLine("버킷이 split 되었습니다.");
                //디렉토리 증가가 필요 없다면, 버킷만 split되었다고 출력

                directory[records[0].realbit].record1 = records[0];
                //옮겨놓은 레코드를 순서에 맞추어 차근차근 넣는다.

                if (directory[records[1].realbit].record1 == null)
                    directory[records[1].realbit].record1 = records[1];
                else
                    directory[records[1].realbit].record2 = records[1];

                if (directory[records[2].realbit].record1 == null)
                    directory[records[2].realbit].record1 = records[2];
                else if (directory[records[2].realbit].record2 == null)
                    directory[records[2].realbit].record2 = records[2];
                else
                    directory[records[2].realbit].record3 = records[2];

                if (directory[A.realbit].record1 == null)
                    directory[A.realbit].record1 = A;
                else if (directory[A.realbit].record2 == null)
                    directory[A.realbit].record2 = A;
                else if (directory[A.realbit].record3 == null)
                    directory[A.realbit].record3 = A;
                else
                    NewRecord(A, step + 1, directory[A.realbit].record1.realbit);
                //만약 단계를 진행함에도 버킷이 가득찼다면, 한번 더 진행한다.
            }

            public void Check()
            {
                for (int i = 0; i < length; i++)                //레코드를 체크한다.
                {
                    if (directory[i].record3 != null)
                    {
                        Console.WriteLine("{0}번 인덱스 디렉토리", Convert.ToString(i, 2));
                        Console.WriteLine("들어간 레코드 : {0}", Convert.ToString(directory[i].record1.record_address, 2));
                        Console.WriteLine("들어간 레코드 : {0}", Convert.ToString(directory[i].record2.record_address, 2));
                        Console.WriteLine("들어간 레코드 : {0}", Convert.ToString(directory[i].record3.record_address, 2));
                    }
                    else if (directory[i].record2 != null)
                    {
                        Console.WriteLine("{0}번 인덱스 디렉토리", Convert.ToString(i, 2));
                        Console.WriteLine("들어간 레코드 : {0}", Convert.ToString(directory[i].record1.record_address, 2));
                        Console.WriteLine("들어간 레코드 : {0}", Convert.ToString(directory[i].record2.record_address, 2));
                    }
                    else if (directory[i].record1 != null)
                    {
                        Console.WriteLine("{0}번 인덱스 디렉토리", Convert.ToString(i, 2));
                        Console.WriteLine("들어간 레코드 : {0}", Convert.ToString(directory[i].record1.record_address, 2));
                    }
                }
            }
        }


        public class Bucket
        {
            public Record record1, record2, record3;        //버킷 안에 저장 될 3개의 레코드

            public Bucket()
            {
                record1 = record2 = record3 = null;         //null로 초기화
            }

            public void Overflow(Bucket over, Record A)
            //이름이 동일한 레코드가 들어갈 때 새로운 버킷을 생성하여 저장한다.
            {
                Console.WriteLine("이름({0})이 중복되어 오버플로 버킷으로 들어갑니다.", A.name);
                if (over.record1 == null)
                    over.record1 = A;
                else if (over.record2 == null)
                    over.record2 = A;
                else if (over.record3 == null)
                    over.record3 = A;
                else
                    Overflow(over.record1.overflow, A);     //그 버킷이 가득차면 재귀적으로 반복
            }

            public void AddRecord(Record A, Directory D)
            {
                if (record1 == null)                            //순서에 맞춰 차근차근 넣어준다.
                    record1 = A;
                else if (record1.name == A.name)  //만약 이름이 동일하다면, Overflow 버킷에 넣어준다.
                    Overflow(record1.overflow, A);
                else if (record2 == null)
                    record2 = A;
                else if (record2.name == A.name)
                    Overflow(record2.overflow, A);
                else if (record3 == null)
                    record3 = A;
                else if (record3.name == A.name)
                    Overflow(record3.overflow, A);
                else
                    D.NewRecord(A, record1.step, record1.realbit);
                //만약 버킷이 가득 찬 상태라면 버킷의 분리를 실시, 메소드는 디렉토리 클래스에 존재한다.
            }
        }

        public class Record                     //레코드 클래스
        {
            public string name;
            public string address;
            public string major;
            public string phone;
            char[] bkey;                        //byte형식의 키값
            uint ikey = 0;                           //int로 수정한 키값
            public uint record_address;          //레코드가 저장될 주소
            public uint bit;                     //한 비트를 나타낼때 사용
            public uint realbit;//레코드가 디렉토리에 실제로 저장될 주소(뒤쪽 비트부터 끊어지는 주소)
            public int step = 0;                //비트연산 단계

            public Bucket overflow = new Bucket();

            public Record(string a, string b, string c, string d)   //초기 레코드 선언
            {
                name = a;
                address = b;
                major = c;
                phone = d;

                Hashing();                                          //해싱을 진행

                this.bit_computation(0);               //0번째 비트를 구한다. 이 때 bit값이 계산됨
                this.realbit = bit;
            }

            public void Hashing()                                   //해싱함수
            {
                bkey = name.ToCharArray();                          //각각의 이름을 Char배열에 저장
                for (int i = 0; i < bkey.Length; i++)
                    ikey += bkey[i];                                //각 자릿수의 이름을 더한다.
                this.record_address = ikey % 67;                    //제수로 모듈러 연산한다. 제수는 67
            }

            public void bit_computation(int a)
            {
                this.bit = (record_address & (uint)(1 << a)) >> a;        //a번째 bit를 구한다. 시작은 0번째
                step++;                                             //단계 1 증가
            }

            public void bit_combination()                           //realbit를 구하는 과정
            {
                bit_computation(step);
                //현재 레코드에 저장된 단계의 bit 계산
                this.realbit += ((uint)Math.Pow(2, step - 1) * bit);
                //realbit 계산(원래 realbit에서 추가로 구한 bit값을 더한다.)
            }

            public void all_bit_combination(int a)      //한번에 주어진 단계까지 계산하기 위해 사용
            {
                realbit = 0;
                step = 0;
                for (int i = 0; i < a; i++)          //0단계부터 bit_combination()을 실시한다.
                    bit_combination();
            }
        }


        static void Main(string[] args)
        {
            Directory dt = new Directory();
            Record[] rc = new Record[50];
            rc[0] = new Record("김대현", "서울시 동대문구", "컴퓨터과학부", "010-7661-7037");                //50개의 레코드!!!!
            rc[1] = new Record("김대현", "노원구", "전기전자컴퓨터", "010-7661-7037");                       //초기에 동일한 이름의 레코드를 생성하여
            rc[2] = new Record("이상엽", "관악구", "기계공학과", "010-2356-7481");                           //동일한 레코드일 경우 오버플로가 정상 작동하는지 확인
            rc[3] = new Record("이동철", "부산", "세무학과", "010-9851-5467");
            rc[4] = new Record("육칠팔", "대전", "사회복지학과", "010-4354-5435");
            rc[5] = new Record("구십디", "대구", "경영학부", "010-1298-1376");
            rc[6] = new Record("이상엽", "창원", "화학공학과", "010-8462-4612");
            rc[7] = new Record("다부고", "마산", "기계정보공학과", "010-9841-6354");
            rc[8] = new Record("사유루", "화성", "환경원예학과", "010-7864-7896");
            rc[9] = new Record("홍찬표", "서울", "중어중문학과", "010-1235-1351");
            rc[10] = new Record("김건호", "금평구", "통계학과", "010-435-4315");
            rc[11] = new Record("사라아", "청원", "물리학과", "010-7611-3555");
            rc[12] = new Record("가나다", "기사", "건축학부", "010-1308-7000");
            rc[13] = new Record("마바사", "달", "건축공학과", "010-0985-0545");
            rc[14] = new Record("일이삼", "금성", "도시공학과", "010-0135-5435");
            rc[15] = new Record("김대현", "전주", "스포츠과학과", "010-7864-1355");
            rc[16] = new Record("니아다", "신라", "융합전공학부", "010-1680-3541");
            rc[17] = new Record("사아가", "고구려", "자유전자", "010-9874-5341");
            rc[18] = new Record("고기오", "발해", "도시행정학과", "010-9875-1357");
            rc[19] = new Record("이거왜", "중국", "음악학과", "010-3970-4315");
            rc[20] = new Record("이러다", "일본", "미술학과", "010-0465-9870");
            rc[21] = new Record("많은거", "미국", "산업디자인과", "010-3344-5540");
            rc[22] = new Record("레고드", "러시아", "환경조각학과", "010-7870-5406");
            rc[23] = new Record("나이르", "태국", "세무학과", "010-9870-6540");
            rc[24] = new Record("사우라", "동작구", "컴퓨터과학부", "010-3750-6540");
            rc[25] = new Record("오시시", "부산", "융합전공학부", "010-780-7007");
            rc[26] = new Record("가개그", "부산", "경제학부", "010-1350-3540");
            rc[27] = new Record("센데이", "창원", "토목공학과", "010-9870-6540");
            rc[28] = new Record("거슬프", "명왕성", "컴퓨터과학부", "010-6570-3548");
            rc[29] = new Record("코드드", "블랙홀", "공간정보공학과", "010-7455-8888");
            rc[30] = new Record("열심하", "일본", "생명과학과", "010-465-7887");
            rc[31] = new Record("짜나드", "가나", "국사학과", "010-7045-1325");
            rc[32] = new Record("거누아", "아시아", "철학과", "010-5466-5433");
            rc[33] = new Record("내코드", "아프리카", "법학과", "010-7898-8700");
            rc[34] = new Record("김대현", "유럽", "조경학과", "010-0404-8788");
            rc[35] = new Record("갓다고", "동유럽", "음악학과", "010-8798-6540");
            rc[36] = new Record("다시아", "북유럽", "오징어잡이과", "010-4505-5450");
            rc[37] = new Record("레어트", "가사나", "감귤포장학과", "010-9876-6870");
            rc[38] = new Record("케이럴", "천정", "기계공학과", "010-3575-3540");
            rc[39] = new Record("수이서", "아제로스", "영문학과", "010-8708-8999");
            rc[40] = new Record("새벼가", "스타크", "컴퓨터과학부", "010-6405-5406");
            rc[41] = new Record("지여심", "전농동", "전기전자컴퓨터", "010-0546-9873");
            rc[42] = new Record("히치다", "우리집", "통계학과", "010-7777-7777");
            rc[43] = new Record("데바데", "다른집", "도시행정학과", "010-8888-8888");
            rc[44] = new Record("난트하", "아무곳", "경영학부", "010-1111-1111");
            rc[45] = new Record("고지다", "11번가", "컴퓨터과학부", "010-999-9999");
            rc[46] = new Record("동무리", "7번방", "생명공학과", "010-3456-6541");
            rc[47] = new Record("수고나", "중국", "국사학과", "010-080-4541");
            rc[48] = new Record("개가도", "케나다", "철학과", "010-7987-4157");
            rc[49] = new Record("조버다", "이집트", "물리학과", "010-740-5407");


            for (int i = 0; i < 50; i++)
                dt.directory[rc[i].realbit].AddRecord(rc[i], dt);

            dt.Check();
        }
    }
}