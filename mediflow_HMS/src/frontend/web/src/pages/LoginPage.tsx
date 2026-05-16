export function LoginPage() {
  const login = () => {
    localStorage.setItem("mf_token", "seed-token");
    window.location.href = "/dashboard";
  };

  return (
    <div className="space-y-4">
      <h2 className="text-2xl font-bold">Sign in to Mediflow</h2>
      <p className="text-sm text-slate-600">Baseline auth screen for project foundation phase.</p>
      <button
        onClick={login}
        className="w-full rounded-lg bg-slate-900 px-4 py-2 font-medium text-white hover:bg-slate-700"
      >
        Continue
      </button>
    </div>
  );
}
