#include "common.hpp"
#include "utils/url_parser.hpp"

int main(int argc, char** argv)
{
	try
	{
		std::string path = argv[0];
		std::string dir = path.substr(0, path.find_last_of("\\/"));

		std::string roblox_app = dir + "\\RobloxPlayerBeta.exe";

		auto args = std::string_view(argv[1]) | std::views::split('+');

		std::vector<std::string> args_vec;
		for (auto arg : args)
		{
			std::string arg_str = std::string(arg);
			if (arg_str.find(":") != std::string::npos)
			{
				arg_str = arg_str.substr(arg_str.find(":") + 1);
			}
			args_vec.push_back(arg_str);
		}

		std::string roblox_args = std::format(
			"--InBrowser -t {} -j \"{}\" -b {} --launchtime={} --rloc {} --gloc {} -channel zflag",
			args_vec[2],
			url_parser::url_decode(args_vec[4]),
			args_vec[5],
			args_vec[3],
			args_vec[6],
			args_vec[7]
		);

		STARTUPINFOA si;
		PROCESS_INFORMATION pi;
		
		ZeroMemory(&si, sizeof(si));
		si.cb = sizeof(si);
		ZeroMemory(&pi, sizeof(pi));
		
		if (CreateProcessA(roblox_app.c_str(), (LPSTR)roblox_args.c_str(), NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi))
		{
			CloseHandle(pi.hProcess);
			CloseHandle(pi.hThread);
		}
		else
		{
			std::cout << "Failed to create process: " << GetLastError() << std::endl;
			return 1;
		}
	}
	catch (const std::exception& ex)
	{
		std::cout << "[ERROR]: " << ex.what() << std::endl;
	}

	return 0;
}