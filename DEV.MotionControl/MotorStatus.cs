using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.MotionControl
{
    public struct MotorStatus
    {
        /// <summary>
        /// 비트 23 모터 활성화됨: Ixx00이 1이고 모터 계산이 활성화된 경우 이 비트는 1이고
        /// Ixx00이 0이고 모터 계산이 비활성화된 경우 이 비트는 0입니다.
        /// </summary>
        public bool MotorActivated { get; set; }
        /// <summary>
        /// 비트 22 음수 끝 제한 설정됨: 모터 실제 위치가 소프트웨어 음수 위치 제한(Ixx14)보다 작거나 이 끝
        /// 부분의 하드웨어 제한(Turbo PMAC의 +LIMn)이 트립된 경우 이 비트는 1이고 그렇지 않은 경우
        /// 0입니다.모터가 비활성화(첫 번째 모터 상태 단어의 비트 23이 0으로 설정됨) 또는 중지(첫 번째 모터
        /// 상태 단어의 비트 19가 0으로 설정됨)된 경우 이 비트는 업데이트되지 않습니다.
        /// </summary>
        public bool NegativeEndLimitSet { get; set; }

        /// <summary>
        /// 비트 21 양수 끝 제한 설정됨: 모터 실제 위치가 소프트웨어 양수 위치 제한(Ixx13)보다 크거나 이 끝
        /// 부분의 하드웨어 제한(-LIMn)이 트립된 경우 이 비트는 1이고
        /// 그렇지 않은 경우 0입니다. 모터가 비활성화(첫 번째 모터 상태 단어의 비트 23이 0으로 설정됨) 또는
        /// 중지(두 번째 모터 상태 단어의 비트 14가 0으로 설정됨)된 경우 이 비트는 업데이트되지 않습니다.
        /// </summary>
        public bool PositiveEndLimitSet { get; set; }

        /// <summary>
        /// 비트 20 확장된 서보 알고리즘 활성화됨: 모터의 Iyy00/Iyy50이 1로 설정되고 모터의 확장된 서보
        /// 알고리즘이 선택된 경우 이 비트는 1입니다.Iyy00/Iyy50이 0이고 PID 서보 알고리즘이 선택된 경우
        /// 이 비트는 0입니다.
        /// </summary>
        public bool HandwheelEnabled { get; set; }

        /// <summary>
        /// 비트 19 앰프 활성화됨: 개-루프(open-loop) 또는 폐-루프(closed-loop) 모드(두 경우를 구별하려면
        /// 개-루프 모드 상태 비트 참조)에서 이 모터의 앰프에 대한 출력이 활성화된 경우 이 비트는 1입니다.
        /// 출력이 비활성화(중지)된 경우 이 비트는 0입니다.
        /// </summary>
        public bool PhasedMotor { get; set; }

        /// <summary>
        /// 비트 18 개-루프(open-loop) 모드: 출력이 활성화 또는 비활성화(중지)된 상태에서 모터의 서보
        /// 루프가 열린 경우 이 비트는 1입니다. (두 경우를 구별하려면 앰프 활성화됨 상태 비트를 참조하십시오.)
        /// 서보 루프가 닫힌 경우(출력이 항상 활성화된 상태에서 위치 제어를 통해) 이 비트는 0입니다.
        /// </summary>
        public bool OpenLoopMode { get; set; }

        /// <summary>
        /// 비트 17 이동 타이머 활성: 미리 정의된 종료 지점 및 종료 시간을 가진 임의의 이동을 모터에서 실행
        /// 중인 경우 이 비트는 1입니다. 여기에는 모든 모션 프로그램 이동 일시 정지 또는 지연, 모든 조그
        /// 위치 이동, 트리거를 찾은 후의 원점 검색 이동 중 일부가 포함됩니다. 그렇지 않은 경우 이 비트는
        /// 0입니다. 명령된 이동의 실행이 끝날 경우 이 비트는 1에서 0으로 변경됩니다.
        /// </summary>
        public bool RunningProgram { get; set; }



    }
}
