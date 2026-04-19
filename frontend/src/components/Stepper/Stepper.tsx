import { useNavigate } from 'react-router-dom';

type StepperProps = {
  currentStep: number;
};

const steps = ['Select rooms', 'Schedule cleaning', 'Process Payment', 'Complete'];

export default function Stepper({ currentStep }: StepperProps) {
  const navigate = useNavigate();
  const progressWidth = `${Math.max(0, Math.min(100, ((currentStep - 1) / 3) * 100))}%`;

  return (
    <section className="rounded border border-slate-200 bg-white px-6 py-6 shadow-sm">
      <div className="mb-4 flex items-center justify-between text-sm text-slate-500">
        <span>Step {currentStep} of 4</span>
        <span>{steps[currentStep - 1]}</span>
      </div>

      <div className="relative mb-8 h-1 w-full rounded bg-slate-200">
        <div className="absolute left-0 top-0 h-1 rounded bg-black" style={{ width: progressWidth }} />
      </div>

      <div className="grid grid-cols-4 gap-2 text-center sm:gap-4">
        {steps.map((step, index) => {
          const stepNumber = index + 1;
          const isCurrent = stepNumber === currentStep;
          const isComplete = stepNumber < currentStep;

          return (
            <button
              key={step}
              type="button"
              onClick={() => navigate(`/cleaning/step/${stepNumber}`)}
              className="flex flex-col items-center gap-3"
            >
              <div
                className={`flex h-10 w-10 items-center justify-center rounded-full text-sm font-medium ${
                  isComplete ? 'bg-emerald-500 text-white' : isCurrent ? 'bg-blue-600 text-white' : 'bg-slate-200 text-slate-500'
                }`}
              >
                {isComplete ? '✓' : stepNumber}
              </div>
              <span className={`text-sm ${isCurrent ? 'font-medium text-slate-900' : 'text-slate-500'}`}>{step}</span>
            </button>
          );
        })}
      </div>
    </section>
  );
}